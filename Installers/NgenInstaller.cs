using System;
using System.Collections.Generic;
using System.Linq;

namespace Paya.Automation.Editor.Installers
{
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using NLog;

    [RunInstaller(true)]
    public partial class NgenInstaller : Installer
    {
        #region Static Fields

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        public NgenInstaller()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     An override function that is called when a MSI installer is run
        ///     and the executable is set as a custom action to be run at install time.
        ///     The native image generator (NGEN.exe) is called, which provides a
        ///     startup performance boost on Windows Forms programs. It is helpful to
        ///     comment out the NGEN code and see how the performance
        ///     is affected.
        /// </summary>
        /// <param name="stateSaver">No need to change this.</param>
        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);

                // get the .NET runtime string, and add the ngen exe at the end.
                string runtimeStr = RuntimeEnvironment.GetRuntimeDirectory();

                if (_Logger.IsInfoEnabled)
                    _Logger.Info("The runtime directory is {0}", runtimeStr);

                string ngenStr = Path.Combine(runtimeStr, "ngen.exe");

                // create a new process...
                Process process = new Process {StartInfo = {FileName = ngenStr, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false}};

                // get the assembly (exe) path and filename.
                string assemblyPath = this.Context.Parameters["assemblypath"];

                if (_Logger.IsInfoEnabled)
                    _Logger.Info("The assembly path is {0}", assemblyPath);

                // add the argument to the filename as the assembly path.
                // Use quotes--important if there are spaces in the name.
                // Use the "install" verb and ngen.exe will compile all deps.
                process.StartInfo.Arguments = "install \"" + assemblyPath + "\"";

                // start ngen. it will do its magic.
                process.Start();
                process.WaitForExit();

                if (_Logger.IsInfoEnabled)
                    _Logger.Info("the NGEN executed.");
            }
            catch (Exception exp)
            {
                if (_Logger.IsErrorEnabled)
                    _Logger.Error(exp, "Error while running creating native images.");
            }
        }

        #endregion
    }
}
