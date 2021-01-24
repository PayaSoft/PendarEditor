using System;
using System.Linq;

namespace Paya.Automation.Editor.Updater
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using ICSharpCode.SharpZipLib.Zip;
    using JetBrains.Annotations;
    using NLog;

    public sealed class PayaApplicationUpdater
    {
        #region Static Fields

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        private readonly string _baseFolder;

        #endregion

        #region Constructors and Destructors

        public PayaApplicationUpdater([NotNull] string baseFolder)
        {
            if (baseFolder == null)
                throw new ArgumentNullException("baseFolder");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            this._baseFolder = baseFolder;
        }

        #endregion

        #region Public Properties

        public string BaseFolder
        {
            get { return this._baseFolder; }
        }

        #endregion

        #region Public Methods and Operators

        public async Task DeleteOriginalFilesAsync()
        {
            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Deleting original files");

            try
            {
                foreach (var file in Directory.EnumerateFiles(this._baseFolder, "*.orig", SearchOption.AllDirectories))
                {
                    try
                    {
                        var deletingFile = file;
                        await TaskEx.Run(() =>
                        {
                            if (File.Exists(deletingFile))
                                File.Delete(deletingFile);
                        });
                    }
                    catch (Exception exp)
                    {
                        if (_Logger.IsWarnEnabled)
                            _Logger.Warn(exp, "Error while cleaning old original file: {0}", file);
                    }
                }
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while cleaning original files.");
            }

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Temporary original files deleted.");
        }

        public async Task<bool> PerformUpdateAsync([NotNull] ZipFile updatePackage, CancellationTokenSource cancellationTokenSource)
        {
            if (updatePackage == null)
                throw new ArgumentNullException("updatePackage");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Updating {0}", this._baseFolder);

            bool anyFileChanged = false;

            if (updatePackage.Count > 0)
            {
                if (!updatePackage.TestArchive(
                                               true,
                                               TestStrategy.FindFirstError,
                                               (status, message) =>
                                                   {
                                                       if (message == null)
                                                       {
                                                           if (_Logger.IsDebugEnabled)
                                                               _Logger.Info("Testing archive: BytesTested={0}; ErrorCount={1}; File={2}; Operation={3};", status.BytesTested, status.ErrorCount, status.File.Name, status.Operation);
                                                       }
                                                       else
                                                       {
                                                           if (status.ErrorCount == 0)
                                                           {
                                                               if (_Logger.IsInfoEnabled)
                                                                   _Logger.Info("Testing archive: {4}; BytesTested={0}; ErrorCount={1}; File={2}; Operation={3};", status.BytesTested, status.ErrorCount, status.File.Name, status.Operation, message);
                                                           }
                                                           else
                                                           {
                                                               if (_Logger.IsWarnEnabled)
                                                                   _Logger.Warn("Testing archive: {4}; BytesTested={0}; ErrorCount={1}; File={2}; Operation={3};", status.BytesTested, status.ErrorCount, status.File.Name, status.Operation, message);
                                                           }
                                                       }
                                                   }))
                {
                    if (_Logger.IsWarnEnabled)
                        _Logger.Warn("Testing update package failed. Update canceled.");

                    return false;
                }

                if (_Logger.IsInfoEnabled)
                    _Logger.Info("The update package tested successfully");

                foreach (var entry in updatePackage.Cast<ZipEntry>().Where(entry => entry != null && entry.IsFile && entry.CanDecompress))
                {
                    bool fileMoved = false;
                    string tmpFileName = null;

                    string fileName = entry.Name;
                    string entryPath = Path.Combine(this._baseFolder, fileName);

                    try
                    {
                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("Extracting file {0}", fileName);

                        byte[] content;
                        using (var stream = updatePackage.GetInputStream(entry))
                        {
                            content = await stream.ReadAllBytesAsync(cancellationTokenSource: cancellationTokenSource);
                        }

                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("Renaming current file {0}", entryPath);
                        fileMoved = this.RenameOriginalFile(entryPath, out tmpFileName);

                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("Updating file {0}", fileName);

                        await Utility.WriteAllBytesAsync(entryPath, content, cancellationTokenSource: cancellationTokenSource);

                        anyFileChanged = true;

                        if (_Logger.IsDebugEnabled)
                            _Logger.Debug("The file {0} updated successfully.", fileName);
                    }
                    catch (Exception exp)
                    {
                        if (_Logger.IsErrorEnabled)
                            _Logger.Error(exp, "Error while updating file {0}", entry.Name);

                        if (!fileMoved || tmpFileName == null)
                            continue;

                        RestoreOriginalFile(entryPath, tmpFileName);
                    }
                }
            }
            else
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn("No files to update!");
            }

            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Update done");

            return anyFileChanged;
        }

        #endregion

        #region Methods

        private static void RestoreOriginalFile(string entryPath, string tmpFileName)
        {
            try
            {
                if (File.Exists(entryPath))
                    File.Delete(entryPath);
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error while deleting file {0}", entryPath);
            }

            try
            {
                File.Move(tmpFileName, entryPath);
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                    _Logger.Warn(exp, "Error while restoring file {0} from {1}", entryPath, tmpFileName);
            }
        }

        private bool RenameOriginalFile(string entryPath, [CanBeNull] out string tmpFileName)
        {
            try
            {
                if (File.Exists(entryPath))
                {
                    if (_Logger.IsTraceEnabled)
                        _Logger.Trace("Generating original file name.");

                    tmpFileName = entryPath + "_" + DateTime.Now.Ticks + ".orig";
                    while (File.Exists(tmpFileName))
                    {
                        string dir = Path.GetDirectoryName(entryPath) ?? this._baseFolder;
                        tmpFileName = Path.Combine(dir, Path.ChangeExtension(Guid.NewGuid().ToString("N"), "orig"));
                    }

                    if (_Logger.IsDebugEnabled)
                        _Logger.Debug("Renaming the original file.");

                    File.Move(entryPath, tmpFileName);

                    return true;
                }

                tmpFileName = null;
                return false;
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while renaming the original file {0}.", entryPath);

                tmpFileName = null;

                return false;
            }
        }

        #endregion
    }
}
