namespace Paya.Automation.Editor.WordEditing
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using NetOffice;
    using NetOffice.WordApi;
    using NetOffice.WordApi.Enums;

    using Task = System.Threading.Tasks.Task;
    using TaskEx = System.Threading.Tasks.Task;

    public class WordEditor : IWordEditor, IDisposable
    {
        private static readonly NLog.ILogger _Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _FileName;

        private readonly WordEditorPermissions _Permissions;

        [NotNull]
        private readonly Models.MessageInfo _MessageInfo;

        private Application _CurrentApp;
        private Document _CurrentDocument;

        private bool _isDisposed; // To detect redundant calls

        private bool _isFinished;

        private bool _isQuit;

        public bool IsSaved { get; private set; }

        private bool _isCloseRequested;

        public WordEditor([NotNull] string fileName, WordEditorPermissions permissions, [NotNull] Models.MessageInfo messageInfo)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");
            if (messageInfo == null)
                throw new ArgumentNullException("fileName");
            global::System.Diagnostics.Contracts.Contract.EndContractBlock();

            this._FileName = fileName;
            this._Permissions = permissions;
            this.Interval = TimeSpan.FromSeconds(0.78534);
            this._MessageInfo = messageInfo;
        }

        public TimeSpan Interval { get; set; }

        public bool IsWaitingForFinish { get; private set; }

        [CanBeNull]
        public byte[] GetContents()
        {
            if (this._CurrentDocument == null)
                return null;

            var tempFile = Path.Combine(Path.GetTempPath(), string.Format(@"{0:N}.docx", Guid.NewGuid()));
            try
            {
                this._CurrentDocument.SaveCopyAs(tempFile);
                return File.ReadAllBytes(tempFile);
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while getting the document content from {0}", tempFile);

                return null;
            }
            finally
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch (Exception exp)
                {
                    if (_Logger.IsWarnEnabled)
                        _Logger.Warn(exp, "Error while deleting Word temp file: {0}", tempFile);
                }
            }
        }

        [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
        public async Task PrintAsync(bool withPreview, CancellationTokenSource cancellationTokenSource = null)
        {
            string fileName = Path.GetFileName(this._FileName);

            if (_Logger.IsTraceEnabled)
                _Logger.Trace("Launching {0}", fileName);

            this._CurrentApp = null;
            this._CurrentDocument = null;

            using (var app = new Application { Visible = true, DisplayAlerts = WdAlertLevel.wdAlertsNone, DisplayRecentFiles = false, Caption = Resources.PendarEditorTitle })
            {
                try
                {
                    app.Console.Mode = DebugConsoleMode.Trace;
                    app.Console.AppendTimeInfoEnabled = true;

                    app.DisplayAlerts = WdAlertLevel.wdAlertsNone;

                    app.DocumentBeforeCloseEvent += this.App_DocumentBeforeCloseEvent;
                    app.DocumentBeforeSaveEvent += this.App_DocumentBeforeSaveEvent;
                    app.DocumentBeforePrintEvent += this.App_DocumentBeforePrintEvent;
                    app.DocumentChangeEvent += this.App_DocumentChangeEvent;
                    app.DocumentOpenEvent += this.App_DocumentOpenEvent;
                    app.NewDocumentEvent += this.App_NewDocumentEvent;
                    app.QuitEvent += this.App_QuitEvent;
                    app.StartupEvent += this.App_StartupEvent;

                    app.Options.AllowReadingMode = true;

                    this._CurrentApp = app;
                    this._isQuit = false;
                    this._isFinished = false;
                    this.IsSaved = false;
                    this.IsWaitingForFinish = false;

                    try
                    {
                        app.Options.ArabicNumeral = WdArabicNumeral.wdNumeralContext;
                        app.Options.BackgroundSave = false;
                        app.Options.CreateBackup = false;
                        app.Options.DocumentViewDirection = WdDocumentViewDirection.wdDocumentViewRtl;
                        app.Options.MeasurementUnit = WdMeasurementUnits.wdMillimeters;
                        app.Options.MonthNames = WdMonthNames.wdMonthNamesFrench;
                        app.Options.DefaultTextEncoding = NetOffice.OfficeApi.Enums.MsoEncoding.msoEncodingUTF8;
                        app.Options.ShowDevTools = false;
                    }
                    catch (COMException exp)
                    {
                        if (_Logger.IsWarnEnabled)
                            _Logger.Warn(exp, "Error while setting the Word options");
                    }

                    using (var doc = app.Documents.Open(this._FileName, true, false, false))
                    {
                        doc.Settings.EnableAutomaticQuit = true;
                        doc.Settings.EnableDebugOutput = Internal.IsDebug;
                        doc.Settings.EnableEventDebugOutput = Internal.IsDebug;
                        doc.Settings.EnableEvents = true;
                        doc.Settings.UseExceptionMessage = ExceptionMessageHandling.CopyInnerExceptionMessageToTopLevelException;
                        doc.Settings.EnableSafeMode = Internal.IsDebug;

                        //try
                        //{
                        //    doc.Final = true;
                        //}
                        //catch (COMException exp)
                        //{
                        //    if (_Logger.IsWarnEnabled)
                        //        _Logger.Warn(exp, "Error while finalizing the document");
                        //}

                        try
                        {
                            doc.Protect(WdProtectionType.wdAllowOnlyFormFields, true, password: Guid.NewGuid().ToString("N"));
                        }
                        catch (COMException exp)
                        {
                            if (_Logger.IsWarnEnabled)
                                _Logger.Warn(exp, "Error while protecting the document");
                        }

                        try
                        {
                            doc.ReadOnlyRecommended = true;
                        }
                        catch (COMException exp)
                        {
                            if (_Logger.IsWarnEnabled)
                                _Logger.Warn(exp, "Error in the ReadOnlyRecommended");
                        }

                        try
                        {
                            doc.Saved = true;
                        }
                        catch (COMException exp)
                        {
                            if (_Logger.IsWarnEnabled)
                                _Logger.Warn(exp, "Error while setting document as saved");
                        }

                        try
                        {
                            if (withPreview)
                            {
                                doc.PrintPreview();

                                app.Activate();

                                await this.WaitForExitAsync(cancellationTokenSource: cancellationTokenSource);
                            }
                            else
                            {
                                doc.PrintOut();
                            }

                            await TaskEx.Delay(90);
                        }
                        finally
                        {
                            try
                            {
                                doc.ClosePrintPreview();
                            }
                            catch (Exception exp)
                            {
                                if (_Logger.IsWarnEnabled)
                                    _Logger.Warn(exp, "Error while closing the print preview");
                            }

                            try
                            {
                                doc.Close(false);
                            }
                            catch (Exception exp)
                            {
                                if (_Logger.IsWarnEnabled)
                                    _Logger.Warn(exp, "Error while closing the word document");
                            }
                        }
                    }
                }
                finally
                {
                    try
                    {
                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("Quiting the word application");

                        if (!this._isQuit)
                            app.Quit(false, true);

                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("Quited the word application");
                    }
                    catch (COMException exp)
                    {
                        if (_Logger.IsWarnEnabled)
                            _Logger.Warn(exp, "Error while quiting the word Application");
                    }
                }
            }
        }

        [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
        public async Task LaunchAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            string fileName = Path.GetFileName(this._FileName);

            if (_Logger.IsTraceEnabled)
                _Logger.Trace("Launching {0}", fileName);

            if (this.IsWaitingForFinish)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn("The document {0} is already launched", fileName);
            }

            this._CurrentApp = null;
            this._CurrentDocument = null;
            this._isCloseRequested = false;

            try
            {

                if (this.Opening != null)
                    this.Opening(this, EventArgs.Empty);

                using (var app = new Application { Visible = true, DisplayAlerts = WdAlertLevel.wdAlertsNone, DisplayRecentFiles = false, Caption = Resources.PendarEditorTitle })
                {
                    try
                    {
                        app.Console.Mode = DebugConsoleMode.Trace;
                        app.Console.AppendTimeInfoEnabled = true;

                        app.DocumentBeforeCloseEvent += this.App_DocumentBeforeCloseEvent;
                        app.DocumentBeforeSaveEvent += this.App_DocumentBeforeSaveEvent;
                        app.DocumentBeforePrintEvent += this.App_DocumentBeforePrintEvent;
                        app.DocumentChangeEvent += this.App_DocumentChangeEvent;
                        app.DocumentOpenEvent += this.App_DocumentOpenEvent;
                        app.NewDocumentEvent += this.App_NewDocumentEvent;
                        app.QuitEvent += this.App_QuitEvent;
                        app.StartupEvent += this.App_StartupEvent;

                        app.Options.AllowReadingMode = false;

                        this._CurrentApp = app;
                        this._isQuit = false;
                        this._isFinished = false;
                        this.IsWaitingForFinish = false;
                        this.IsSaved = false;

                        //try
                        //{
                        //    app.Activate();
                        //}
                        //catch (Exception exp)
                        //{
                        //    if (_Logger.IsWarnEnabled)
                        //        _Logger.Warn(exp, "Error while activating the Word application");
                        //}

                        try
                        {
                            app.KeyboardBidi();
                        }
                        catch (COMException exp)
                        {
                            if (_Logger.IsWarnEnabled)
                                _Logger.Warn(exp, "Error while setting the BIDI keyboard");
                        }

                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("The Word application initialized");

                        using (var doc = app.Documents.Open(this._FileName, true, false, false))
                        {
                            this._CurrentDocument = doc;

                            try
                            {
                                doc.Settings.EnableAutomaticQuit = true;
                                doc.Settings.EnableDebugOutput = Internal.IsDebug;
                                doc.Settings.EnableEventDebugOutput = Internal.IsDebug;
                                doc.Settings.EnableEvents = true;
                                doc.Settings.UseExceptionMessage = ExceptionMessageHandling.CopyInnerExceptionMessageToTopLevelException;
                                doc.Settings.EnableSafeMode = Internal.IsDebug;

                                if (_Logger.IsDebugEnabled)
                                    _Logger.Debug("Word document initialized");

                                if (!this.CanChange)
                                {
                                    if (_Logger.IsInfoEnabled)
                                        _Logger.Info("Read-Only Word document. Protecting...");

                                    try
                                    {
                                        doc.Final = true;
                                    }
                                    catch (COMException exp)
                                    {
                                        if (_Logger.IsWarnEnabled)
                                            _Logger.Warn(exp, "Error while finalizing the Word document. Maybe the Word is not activated.");
                                    }

                                    try
                                    {
                                        doc.Protect(WdProtectionType.wdAllowOnlyFormFields, true, password: Guid.NewGuid().ToString("N"));
                                    }
                                    catch (COMException exp)
                                    {
                                        if (_Logger.IsWarnEnabled)
                                            _Logger.Warn(exp, "Error while protecting the Word document. Maybe the Word is not activated.");
                                    }

                                    try
                                    {
                                        doc.Saved = true;
                                    }
                                    catch (COMException exp)
                                    {
                                        if (_Logger.IsWarnEnabled)
                                            _Logger.Warn(exp, "Error while setting document as saved");
                                    }
                                }
                            }
                            finally
                            {
                                if (_Logger.IsTraceEnabled)
                                    _Logger.Trace("Activating the Word window.");

                                //await TaskEx.Delay(50); // not available in C# 5.0

                                app.Activate();
                                //doc.Activate();

                                if (_Logger.IsTraceEnabled)
                                    _Logger.Trace("The Word application activated");
                            }

                            if (this.CanChange)
                            {
                                this.IsWaitingForFinish = true;

                                if (_Logger.IsDebugEnabled)
                                    _Logger.Debug("Waiting for the Word document to close");

                                await this.WaitForExitAsync(() => this._MessageInfo.CloseRequested, cancellationTokenSource);

                                if (this._isCloseRequested || cancellationTokenSource.IsCancellationRequested)
                                {
                                    _Logger.Warn("Saving Word document canceled");

                                    this.IsSaved = false;
                                }

                                if (_Logger.IsInfoEnabled)
                                    _Logger.Info("The Word document is closed");

                                this.IsWaitingForFinish = false;
                            }
                            else
                            {
                                try
                                {
                                    app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
                                }
                                catch (Exception exp)
                                {
                                    if (_Logger.IsWarnEnabled)
                                        _Logger.Warn(exp, "Error preventing alerts to display");
                                }

                                this.IsSaved = false;
                            }
                        }
                    }
                    finally
                    {
                        if (!this._isQuit && this.CanChange)
                        {
                            try
                            {
                                if (_Logger.IsDebugEnabled)
                                    _Logger.Debug("Quiting the Word application");

                                app.Quit(false, true);

                                if (_Logger.IsDebugEnabled)
                                    _Logger.Debug("Quited the Word application");
                            }
                            catch (COMException exp)
                            {
                                if (_Logger.IsWarnEnabled)
                                    _Logger.Warn(exp, "Error while quiting the Word application");
                            }
                        }
                    }
                }
            }
            finally
            {
                this._CurrentDocument = null;
                this._CurrentApp = null;
            }

            if (this.Finished != null)
                this.Finished(this, EventArgs.Empty);

            if (_Logger.IsInfoEnabled)
                _Logger.Info("Launching Word finished");
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(Boolean disposing) above.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected async Task WaitForExitAsync([CanBeNull] Func<bool> closeRequested = null, [CanBeNull] CancellationTokenSource cancellationTokenSource = null)
        {
            while ((!this._isFinished || !this._isQuit) && !this._isDisposed)
            {
                if (closeRequested != null)
                {
                    if (closeRequested())
                    {
                        this._isCloseRequested = true;
                        break;
                    }
                }

                if (cancellationTokenSource != null)
                    await TaskEx.Delay(this.Interval, cancellationTokenSource.Token);
                else
                    await TaskEx.Delay(this.Interval);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this._isFinished = true;
                }

                if (this._CurrentDocument != null)
                {
                    this._CurrentDocument.Dispose();
                    this._CurrentDocument = null;
                }

                if (this._CurrentApp != null)
                {
                    if (!this._isQuit)
                    {
                        try
                        {
                            this._CurrentApp.Quit(false);
                        }
                        catch (COMException)
                        {
                            // ignore
                        }
                    }

                    this._CurrentApp.Dispose();
                }

                this._isDisposed = true;
            }
        }

        private void App_StartupEvent()
        {
            if (this.Startup != null)
                this.Startup(this, EventArgs.Empty);

            if (_Logger.IsInfoEnabled)
                _Logger.Info("Word document startup event");
        }

        private void App_QuitEvent()
        {
            this._isQuit = true;

            if (this.Quit != null)
                this.Quit(this, EventArgs.Empty);

            if (_Logger.IsInfoEnabled)
                _Logger.Info("Word document quit event");
        }

        private void App_NewDocumentEvent(Document Doc)
        {
            if (this.NewDocument != null)
                this.NewDocument(this, EventArgs.Empty);

            if (_Logger.IsInfoEnabled)
                _Logger.Info("Word document new event");
        }

        private void App_DocumentOpenEvent(Document Doc)
        {
            if (this.Opened != null)
                this.Opened(this, EventArgs.Empty);

            if (_Logger.IsInfoEnabled)
                _Logger.Info("Word document open event");
        }

        private void App_DocumentChangeEvent()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Word document change event");
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        private void App_DocumentBeforePrintEvent(Document Doc, ref bool cancel)
        {
            cancel = !this.CanPrint;

            if (!cancel)
            {
                if (this.Printing != null)
                    this.Printing(this, EventArgs.Empty);
                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Word document print event");
            }
            else
            {
                if (_Logger.IsDebugEnabled)
                    _Logger.Debug("Word document print canceled");
            }


        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private void App_DocumentBeforeSaveEvent(Document Doc, ref bool saveAsUI, ref bool cancel)
        {
            if (!this.CanChange)
            {
                cancel = true;
                return;
            }

            this.IsSaved = true;

            if (this.Saving != null)
                this.Saving(this, EventArgs.Empty);

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Word document save event");
        }

        private void App_DocumentBeforeCloseEvent(Document Doc, ref bool cancel)
        {
            this._isFinished = true;

            if (this.Closed != null)
                this.Closed(this, EventArgs.Empty);

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Word document close event");
        }

        #region IWordEditor Members

        public bool CanChange
        {
            get
            {
                return (this._Permissions & WordEditorPermissions.Change) !=
                       0;
            }
        }

        public bool CanPrint
        {
            get
            {
                return (this._Permissions & WordEditorPermissions.Print) !=
                       0;
            }
        }

        public event EventHandler Opening, Opened, Closed, Changed, Quit, Saving, Startup, NewDocument, Printing, Finished;

        #endregion

        ~WordEditor()
        {
            // Do not change this code. Put cleanup code in Dispose(Boolean disposing) above.
            try
            {
                this.Dispose(false);
            }
            catch
            {
                // ignore
            }
        }
    }
}