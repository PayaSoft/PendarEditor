namespace Paya.Automation.Editor.Logs
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Threading;
    using JetBrains.Annotations;
    using NLog;
    using NLog.Common;
    using NLog.Config;
    using NLog.Targets;
    using Paya.Automation.Editor.Properties;

    [Target("WpfTextBox")]
    [ThreadAgnostic]
    [PublicAPI]
    public sealed class WpfTextBoxTarget : TargetWithLayout
    {
        #region Fields

        private readonly object _syncObject = new object();

        private TextBoxBase _TextBox;

        private Window _Window;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WpfTextBoxTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is:
        ///     <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public WpfTextBoxTarget()
        {
            this.MaxNumberOfLinesToTriggerRemove = Settings.Default.TextBoxLoggerLayoutMaxNumberOfLinesToTriggerRemove;
            this.NumberOfLinesToRemove = Settings.Default.TextBoxLoggerLayoutNumberOfLinesToRemove;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Specifies the <see cref="AppendTextDelegate" /> delegate.
        /// </summary>
        /// <param name="logText">The log text.</param>
        private delegate void AppendTextDelegate(string logText);

        /// <param name="app">The application.</param>
        /// <param name="windowTypeName">Name of the window type.</param>
        /// <returns></returns>
        private delegate Window GetAppWindowDelegate(Application app, string windowTypeName);

        /// <summary>
        ///     Specifies the <see cref="GetTextBoxDelegate" /> delegate.
        /// </summary>
        /// <param name="win">The win.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        private delegate TextBoxBase GetTextBoxDelegate(Window win, string elementName);

        #endregion

        #region Public Properties

        [RequiredParameter]
        [DefaultParameter]
        public string ElementName { get; set; }

        [DefaultValue(1200)]
        public int MaxNumberOfLinesToTriggerRemove { get; set; }

        [DefaultValue(300)]
        public int NumberOfLinesToRemove { get; set; }

        public string WindowTypeName { get; set; }

        #endregion

        #region Properties

        [CanBeNull]
        private TextBoxBase TextBox
        {
            get
            {
                if (Application.Current == null)
                    return null;

                return this._TextBox ?? (this._TextBox = (TextBoxBase)Application.Current.Dispatcher.Invoke(new GetTextBoxDelegate((win, eln) => win != null ? win.FindName(eln) as TextBoxBase : null), this.TheWindow, this.ElementName));
            }
        }

        private Window TheWindow
        {
            get
            {
                return (Window)Application.Current.Dispatcher.Invoke(new GetAppWindowDelegate((app, wtn) =>
                    {
                        if (this._Window == null)
                        {
                            if (string.IsNullOrWhiteSpace(wtn))
                                this._Window = app.MainWindow;
                            else
                                this._Window = (from w in app.Windows.OfType<Window>()
                                                where w != null
                                                let type = w.GetType()
                                                where StringComparer.OrdinalIgnoreCase.Equals(type.FullName, wtn)
                                                      || StringComparer.OrdinalIgnoreCase.Equals(type.Name, wtn)
                                                      || StringComparer.OrdinalIgnoreCase.Equals(w.Name, wtn)
                                                select w).FirstOrDefault();

                            if (this._Window == null)
                            {
                                if (InternalLogger.IsErrorEnabled)
                                    InternalLogger.Error("The window of type {0} not found.", wtn);
                            }
                        }

                        return this._Window;
                    }), Application.Current, this.WindowTypeName);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; <c>false</c> to release only unmanaged
        ///     resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            this._TextBox = null;
            this._Window = null;

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Writes logging event to the log target.
        ///     classes.
        /// </summary>
        /// <param name="logEvent">Logging event to be written out.</param>
        /// <exception cref="System.ArgumentNullException">logEvent</exception>
        protected override void Write([NotNull] LogEventInfo logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException("logEvent");
            }
            string logText = this.GetLogText(logEvent);
            this.AppendLog(logText, false);
        }

        /// <summary>
        ///     Writes log event to the log target. Must be overridden in inheriting
        ///     classes.
        /// </summary>
        /// <param name="logEvent">Log event to be written out.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            string logText = this.GetLogText(logEvent.LogEvent);
            this.AppendLog(logText, true);
        }

        /// <summary>
        ///     Writes an array of logging events to the log target. By default it iterates on all
        ///     events and passes them to "Write" method. Inheriting classes can use this method to
        ///     optimize batch writes.
        /// </summary>
        /// <param name="logEvents">Logging events to be written out.</param>
        /// <exception cref="System.ArgumentNullException">logEvents</exception>
        [Obsolete("Instead override Write(IList<AsyncLogEventInfo> logEvents. Marked obsolete on NLog 4.5")]
        protected override void Write([NotNull] AsyncLogEventInfo[] logEvents)
        {
#if DEBUG
            if (logEvents == null)
            {
                throw new ArgumentNullException("logEvents");
            }
#endif

            string logText = logEvents.Aggregate(new StringBuilder(), (sb, logEvent) => sb.Append(this.GetLogText(logEvent.LogEvent)), sb => sb.ToString());
            this.AppendLog(logText, true);
        }

        private void AppendLog([NotNull] string logText, bool async)
        {
            var txt = this.TextBox;
            if (txt == null)
            {
                return;
            }

            //var richTextBox = txt as RichTextBox;
            //if (richTextBox != null)
            //{
            //	txt.Dispatcher.Invoke(new Action(() => richTextBox.Document.Blocks.Add(new Paragraph(new Run(logText) { Foreground = Brushes.Red }))));
            //	return;
            //}

            this.RemoveExtraLines(async);

            Delegate dlg = new AppendTextDelegate(txt.AppendText);


            if (!async)
            {
                txt.Dispatcher.Invoke(dlg, logText);
                txt.Dispatcher.Invoke(new Action(txt.ScrollToEnd));
            }
            else
            {
                txt.Dispatcher.BeginInvoke(dlg, DispatcherPriority.ContextIdle, logText);
                txt.Dispatcher.BeginInvoke(new Action(txt.ScrollToEnd), DispatcherPriority.ApplicationIdle);
            }
        }

        [Pure, NotNull]
        private string GetLogText(LogEventInfo logEvent)
        {
            string str = this.Layout.Render(logEvent);
            if (str.All(c => c < 127))
                str = string.Format(CultureInfo.InvariantCulture, "\u202D{0}\u202C", str);
            return str + Environment.NewLine;
        }

        private void RemoveExtraLines(bool async)
        {
            var textBox = this.TextBox as TextBox;
            if (textBox != null)
            {
                var action = new Action(() =>
                    {
                        if (textBox.LineCount <= this.MaxNumberOfLinesToTriggerRemove)
                            return;

                        lock (this._syncObject)
                        {
                            int s = 0;
                            for (int i = 0; i < this.NumberOfLinesToRemove; i++)
                                s += textBox.GetLineLength(i);
                            string text = textBox.Text;
                            textBox.Text = s >= text.Length ? string.Empty : textBox.Text.Substring(s);
                        }
                    });

                if (async)
                    textBox.Dispatcher.BeginInvoke(action);
                else
                    textBox.Dispatcher.Invoke(action);
            }
        }

        #endregion
    }
}
