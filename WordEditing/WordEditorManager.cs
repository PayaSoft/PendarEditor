namespace Paya.Automation.Editor.WordEditing
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Models;

    using NLog;

    using TaskEx = System.Threading.Tasks.Task;

    public class WordEditorManager : IDisposable, IWordEditor
    {
        [NotNull]
        private static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();

        [NotNull]
        private readonly MessageSessionData _SessionData;

        [CanBeNull]
        private CancellationTokenSource _CancellationTokenSource;

        [CanBeNull]
        private WordEditor _CurrentEditor;

        private bool disposedValue; // To detect redundant calls

        [NotNull]
        private static readonly ObservableCollection<MessageInfo> _Messages = new ObservableCollection<MessageInfo>();

        private static readonly object _MessagesSyncObj = new object();

        public WordEditorManager([NotNull] MessageSessionData sessionData)
        {
            if (sessionData == null)
                throw new ArgumentNullException("sessionData");

            Contract.EndContractBlock();

            this._SessionData = sessionData;
        }

        public bool InsertHeader { get; set; }

        public bool InsertSigns { get; set; }

        public bool InsertSignImage { get; set; }

        public bool InsertCopyText { get; set; }

        public bool InsertRemarks { get; set; }

        [NotNull]
        [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
        public async Task LaunchAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            if (this._CurrentEditor != null)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn("The current editor is not null");

                return;
            }

            this._CancellationTokenSource = cancellationTokenSource;
            try
            {
                var messageInfo = await this._SessionData.GetMessageInfoAsync(cancellationTokenSource);

                if (messageInfo == null)
                {
                    if (_Logger.IsErrorEnabled)
                        _Logger.Error("Error while getting message info");
                    return;
                }

                var isReadOnly = !messageInfo.GetJsonValue<bool>("Permissions.ChangeBody");

                bool needsHeader = !StringComparer.OrdinalIgnoreCase.Equals(messageInfo.GetJsonValue<string>("Class.Code"), @"Varedeh");


                var content = await this._SessionData.GetMessageContentAsync(needsHeader && this.InsertHeader, isReadOnly && this.InsertSigns, isReadOnly && this.InsertSignImage, isReadOnly && this.InsertCopyText, isReadOnly && this.InsertRemarks, cancellationTokenSource);

                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Message content loaded");

                var messageDisplayName = messageInfo.GetJsonValue<string>("DisplayName") ?? Guid.NewGuid().ToString();

                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Message display name is {0}", messageDisplayName);

                var msgInfo = new MessageInfo(this._SessionData, messageDisplayName);

                lock (_MessagesSyncObj)
                {
                    _Messages.Add(msgInfo);
                }


                try
                {

                    if (_Logger.IsDebugEnabled)
                        _Logger.Debug("Creating temp file");

                    var tempFile = GetTempFileName(messageDisplayName);

                    if (_Logger.IsDebugEnabled)
                        _Logger.Debug("The temp file is {0}", tempFile);

                    if (cancellationTokenSource == null)
                        await TaskEx.Run(() => File.WriteAllBytes(tempFile, content));
                    else
                        await TaskEx.Run(() => File.WriteAllBytes(tempFile, content), cancellationTokenSource.Token);

                    if (_Logger.IsInfoEnabled)
                        _Logger.Info("The temp file is {0}", tempFile);

                    try
                    {
                        var fi = new FileInfo(tempFile);

                        Debug.Assert(fi.Exists, "The temp file not found", "File not found: {0}", tempFile);

                        //fi.IsReadOnly = isReadOnly;
                        fi.CreationTime = messageInfo.GetJsonValue<DateTime?>("CreatedAt") ?? DateTime.Now;
                        fi.LastWriteTime = messageInfo.GetJsonValue<DateTime?>("ChangedAt") ?? DateTime.Now;
                    }
                    catch (Exception exp)
                    {
                        if (_Logger.IsWarnEnabled)
                            _Logger.Warn(exp, "Error while setting temp file info");
                    }

                    if (isReadOnly)
                        _Logger.Info("The message is read only");

                    var canPrint = messageInfo.GetJsonValue<bool>("Permissions.Print");

                    using (var editor = new WordEditor(tempFile, (isReadOnly ? WordEditorPermissions.None : WordEditorPermissions.Change) | (canPrint ? WordEditorPermissions.Print : WordEditorPermissions.None), msgInfo))
                    {
                        this._CurrentEditor = editor;

                        await TaskEx.Delay(110);

                        try
                        {
                            Func<Task> launch = async () => await editor.LaunchAsync(cancellationTokenSource).ConfigureAwait(true);

                            await (Task)System.Windows.Application.Current.Dispatcher.Invoke(launch);

                            if (editor.IsSaved)
                            {
                                if (_Logger.IsDebugEnabled)
                                    _Logger.Debug("Editor closed with save");

                                if (File.Exists(tempFile))
                                {
                                    var body = await Utility.ReadAllBytesAsync(tempFile, cancellationTokenSource);

                                    if (body.Length == 0)
                                    {
                                        if (_Logger.IsWarnEnabled)
                                            _Logger.Warn("The current editing file is empty");
                                        return;
                                    }

                                    if (_Logger.IsDebugEnabled)
                                        _Logger.Debug("Uploading the message body");

                                    var result = await this._SessionData.SetBodyAsync(body, cancellationTokenSource: cancellationTokenSource);

                                    if (result != null && !result.IsSuccess)
                                    {
                                        if (_Logger.IsErrorEnabled)
                                            _Logger.Error("Error while uploading the message body to the server: {0}", result.Message);
                                    }
                                }

                                return;
                            }
                            else
                            {
                                if (_Logger.IsDebugEnabled)
                                    _Logger.Debug("Editor closed with no save");

                                return;
                            }
                        }
                        catch (Exception exp)
                        {
                            if (_Logger.IsErrorEnabled)
                                _Logger.Error(exp, "Launching error in WordEditor");

                            throw;
                        }
                        finally
                        {
                            this._CurrentEditor = null;
                        }
                    }
                }
                catch (Exception exp)
                {
                    if (_Logger.IsErrorEnabled)
                        _Logger.Error(exp, "Launching error in creation");

                    throw;
                }
                finally
                {
                    lock (_MessagesSyncObj)
                    {
                        _Messages.Remove(msgInfo);
                    }
                }
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Launching error");

                throw;
            }
            finally
            {
                this._CancellationTokenSource = null;
            }
        }

        [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
        public async Task PrintAsync(bool withPreview, CancellationTokenSource cancellationTokenSource = null)
        {
            if (this._CurrentEditor != null)
                return;

            this._CancellationTokenSource = cancellationTokenSource;
            try
            {
                var messageInfo = await this._SessionData.GetMessageInfoAsync(cancellationTokenSource);

                if (messageInfo == null)
                    return;

                var hasPrintPermission = messageInfo.GetJsonValue<bool>("Permissions.Print");
                if (!hasPrintPermission)
                    throw new SecurityException("Print permission denied");

                bool insertHeader;
                if (this.InsertHeader)
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(messageInfo.GetJsonValue<string>("Class.Code"), @"Varedeh"))
                    {
                        insertHeader = false;
                    }
                    else
                        insertHeader = true;
                }
                else
                    insertHeader = false;

                var content = await this._SessionData.GetMessageContentAsync(insertHeader, this.InsertSigns, this.InsertSignImage, this.InsertCopyText, this.InsertRemarks, cancellationTokenSource);

                if (content == null)
                    return;

                var messageDisplayName = messageInfo.GetJsonValue<string>("DisplayName");

                var tempFile = GetTempFileName(messageDisplayName);

                if (cancellationTokenSource == null)
                    await TaskEx.Run(() => File.WriteAllBytes(tempFile, content));
                else
                    await TaskEx.Run(() => File.WriteAllBytes(tempFile, content), cancellationTokenSource.Token);

                try
                {
                    var fi = new FileInfo(tempFile);

                    Debug.Assert(fi.Exists, "The temp file not found", "File not found: {0}", tempFile);

                    //fi.IsReadOnly = true;
                    fi.CreationTime = messageInfo.GetJsonValue<DateTime?>("CreatedAt") ?? DateTime.Now;
                    fi.LastWriteTime = messageInfo.GetJsonValue<DateTime?>("ChangedAt") ?? DateTime.Now;
                }
                catch (Exception exp)
                {
                    if (_Logger.IsWarnEnabled)
                        _Logger.Warn(exp, "Error while setting temp file info");
                }

                using (var editor = new WordEditor(tempFile, WordEditorPermissions.Print, new MessageInfo(this._SessionData, messageDisplayName)))
                {
                    this._CurrentEditor = editor;

                    try
                    {
                        await editor.PrintAsync(withPreview, cancellationTokenSource);

                        //if (File.Exists(tempFile))
                        //{
                        //    var body = await Utility.ReadAllBytesAsync(tempFile, cancellationTokenSource);

                        //    if (body.Length == 0)
                        //    {
                        //        if (_Logger.IsWarnEnabled)
                        //            _Logger.Warn("The current editing file is empty");
                        //        return;
                        //    }

                        //    if (_Logger.IsDebugEnabled)
                        //        _Logger.Debug("Uploading the message body");

                        //    var result = await this._SessionData.SetBodyAsync(body, cancellationTokenSource: cancellationTokenSource);

                        //    if (result != null && !result.IsSuccess)
                        //    {
                        //        if (_Logger.IsErrorEnabled)
                        //            _Logger.Error("Error while uploading the message body to the server: {0}", result.Message);
                        //    }
                        //}
                    }
                    finally
                    {
                        this._CurrentEditor = null;
                    }

                }
            }
            finally
            {
                this._CancellationTokenSource = null;
            }

        }

        internal static void Close([NotNull] MessageInfo messageInfo)
        {
            if (messageInfo == null)
                throw new ArgumentNullException("messageInfo");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            messageInfo.CloseRequested = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(Boolean disposing) above.
            this.Dispose(true);
        }

        [CanBeNull]
        public byte[] GetContents()
        {
            return this._CurrentEditor != null ? this._CurrentEditor.GetContents() : null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this._CancellationTokenSource != null)
                    {
                        try
                        {
                            this._CancellationTokenSource.Cancel(false);
                        }
                        catch (Exception exp)
                        {
                            if (_Logger.IsWarnEnabled)
                                _Logger.Warn(exp, "Error while canceling the current requests");
                        }

                        this._CancellationTokenSource = null;
                    }

                    if (this._CurrentEditor != null)
                    {
                        this._CurrentEditor.Dispose();
                        this._CurrentEditor = null;
                    }
                }

                this.disposedValue = true;
            }
        }

        [MustUseReturnValue]
        [NotNull]
        private static string GetTempFileName([NotNull] string fileName)
        {
            try
            {

                var now = DateTime.Now;

                var dir = Path.Combine(Path.GetTempPath(), string.Format(@"PB-{0:N}\{1}\{2:N}", Guid.NewGuid(), now.Year % 100 + now.Month, Guid.NewGuid()));

                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }
                catch (Exception exp)
                {
                    if (_Logger.IsWarnEnabled)
                        _Logger.Warn(exp, "Error while creating the temp directory {0}", dir);

                    dir = Path.GetTempPath();
                }

                fileName = Utility.ReplaceInvalidFileNameChars(fileName);
                if (fileName.Length > 100)
                    fileName = fileName.Substring(0, 100);

                var path = Path.Combine(dir, string.Format("{0}.docx", fileName));

                return path;
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while getting temp file name for: {0}", fileName);

                return Path.Combine(Path.GetTempPath(), string.Format("{0}.docx", Guid.NewGuid()));
            }
        }



        public bool CanChange { get { return this._CurrentEditor != null && ((IWordEditor)this._CurrentEditor).CanChange; } }





        public bool CanPrint { get { return this._CurrentEditor != null && ((IWordEditor)this._CurrentEditor).CanPrint; } }

        [NotNull, ItemNotNull]
        public static ObservableCollection<MessageInfo> Messages
        {
            get
            {
                lock (_MessagesSyncObj)
                {
                    return _Messages;
                }
            }
        }





        public event EventHandler Changed
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Changed += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Changed -= value;
            }
        }





        public event EventHandler Closed
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Closed += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Closed -= value;
            }
        }





        public event EventHandler Finished
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Finished += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Finished -= value;
            }
        }





        public event EventHandler NewDocument
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.NewDocument += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.NewDocument -= value;
            }
        }





        public event EventHandler Opened
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Opened += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Opened -= value;
            }
        }





        public event EventHandler Opening
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Opening += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Opening -= value;
            }
        }





        public event EventHandler Printing
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Printing += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Printing -= value;
            }
        }





        public event EventHandler Quit
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Quit += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Quit -= value;
            }
        }





        public event EventHandler Saving
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Saving += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Saving -= value;
            }
        }





        public event EventHandler Startup
        {
            add
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Startup += value;
            }
            remove
            {
                if (this._CurrentEditor != null)
                    this._CurrentEditor.Startup -= value;
            }
        }


    }
}