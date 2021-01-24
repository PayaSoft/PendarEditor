namespace Paya.Automation.Editor
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Hardcodet.Wpf.TaskbarNotification;
    using JetBrains.Annotations;
    using MahApps.Metro;
    using MahApps.Metro.Controls.Dialogs;
    using Microsoft.Win32;
    using NLog;
    using Paya.Cryptography;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    [CLSCompliant(false)]
    public partial class MainWindow
    {
        #region Constants

        /// <summary>
        ///     The <see cref="Accents" /> dependency property's name.
        /// </summary>
        public const string AccentsPropertyName = "Accents";

        /// <summary>
        ///     The <see cref="SelectedAccent" /> dependency property's name.
        /// </summary>
        public const string SelectedAccentPropertyName = "SelectedAccent";

        /// <summary>
        ///     The <see cref="SelectedDirection" /> dependency property's name.
        /// </summary>
        public const string SelectedDirectionPropertyName = "SelectedDirection";

        /// <summary>
        ///     The <see cref="SelectedTheme" /> dependency property's name.
        /// </summary>
        public const string SelectedThemePropertyName = "SelectedTheme";

        /// <summary>
        ///     The <see cref="Themes" /> dependency property's name.
        /// </summary>
        public const string ThemesPropertyName = "Themes";

        private const string RegstryKeyLocation = @"Software\Paya\Paya.Automation.Editor";

        public bool Test = false;
        #endregion

        #region Static Fields

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Identifies the <see cref="Themes" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThemesProperty = DependencyProperty.Register(
                                                                                               ThemesPropertyName,
                                                                                               typeof(ObservableCollection<string>),
                                                                                               typeof(MainWindow),
                                                                                               new UIPropertyMetadata(new ObservableCollection<string>(new[] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" })));

        /// <summary>
        ///     Identifies the <see cref="Accents" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AccentsProperty = DependencyProperty.Register(
                                                                                                AccentsPropertyName,
                                                                                                typeof(ObservableCollection<string>),
                                                                                                typeof(MainWindow),
                                                                                                new UIPropertyMetadata(new ObservableCollection<string>(new[] { "BaseLight", "BaseDark" })));

        /// <summary>
        ///     Identifies the <see cref="SelectedTheme" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedThemeProperty = DependencyProperty.Register(
                                                                                                      SelectedThemePropertyName,
                                                                                                      typeof(string),
                                                                                                      typeof(MainWindow),
                                                                                                      new UIPropertyMetadata("Blue", OnSelectedThemeChanged));

        /// <summary>
        ///     Identifies the <see cref="SelectedAccent" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedAccentProperty = DependencyProperty.Register(
                                                                                                       SelectedAccentPropertyName,
                                                                                                       typeof(string),
                                                                                                       typeof(MainWindow),
                                                                                                       new UIPropertyMetadata("BaseLight", OnSelectedAccentChanged));

        /// <summary>
        ///     Identifies the <see cref="SelectedDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDirectionProperty = DependencyProperty.Register(
                                                                                                          SelectedDirectionPropertyName,
                                                                                                          typeof(FlowDirection),
                                                                                                          typeof(MainWindow),
                                                                                                          new UIPropertyMetadata(FlowDirection.LeftToRight));

        #endregion

        #region Fields

        private CancellationTokenSource _CancellationTokenSource;

        private readonly object _CancellationTokenSourceSyncObj = new object();

        private bool _FormLoaded;

        private OpenMessages _OpenMessages;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <autogeneratedoc />
        public MainWindow()
        {
            if (_Logger.IsTraceEnabled)
                _Logger.Trace("در حال ایجاد پنجره‌ی اصلی");

            this.InitializeComponent();

            this.DirectionsCombobox.DataContext = this.LayoutRoot.DataContext;

            this.Title = "سرویس اتوماسیون اداری پایا (پندار)" + " " + Internal.Version;

            this.Unloaded += (s, e) => this._FormLoaded = false;

            this.Loaded += this.MainWindow_Loaded;

            this.ChangeTheme();

            if (_Logger.IsInfoEnabled)
                _Logger.Info("پنجره‌ی اصلی ایجاد شد");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            const string regpath = @"SOFTWARE\Paya\Pendar\Editor";

            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(regpath, false))
                {
                    if (key != null)
                    {
                        this.EnableAutoUpdate = Convert.ToBoolean(key.GetValue("EnableAutoUpdate"));
                    }
                }
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                {
                    _Logger.Warn(exp, "Error while loading EnableAutoUpdate into the registry");
                }

                this.EnableAutoUpdate = true;
            }
        }

        #endregion

        #region Public Properties

        internal OpenMessages OpenMessages { get { return this._OpenMessages ?? (this._OpenMessages = new OpenMessages()); } }

        /// <summary>
        ///     Gets or sets the value of the <see cref="Accents" />
        ///     property. This is a dependency property.
        /// </summary>
        public ObservableCollection<string> Accents
        {
            get { return (ObservableCollection<string>)this.GetValue(AccentsProperty); }
            set { this.SetValue(AccentsProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="SelectedAccent" />
        ///     property. This is a dependency property.
        /// </summary>
        public string SelectedAccent
        {
            get { return (string)this.GetValue(SelectedAccentProperty); }
            set { this.SetValue(SelectedAccentProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="SelectedDirection" />
        ///     property. This is a dependency property.
        /// </summary>
        public FlowDirection SelectedDirection
        {
            get { return (FlowDirection)this.GetValue(SelectedDirectionProperty); }
            set { this.SetValue(SelectedDirectionProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="SelectedTheme" />
        ///     property. This is a dependency property.
        /// </summary>
        public string SelectedTheme
        {
            get { return (string)this.GetValue(SelectedThemeProperty); }
            set { this.SetValue(SelectedThemeProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="Themes" />
        ///     property. This is a dependency property.
        /// </summary>
        public ObservableCollection<string> Themes
        {
            get { return (ObservableCollection<string>)this.GetValue(ThemesProperty); }
            set { this.SetValue(ThemesProperty, value); }
        }

        #endregion

        #region Public Methods and Operators

        public void ChangeTheme()
        {
            if (_Logger.IsDebugEnabled)
                _Logger.Debug("Changing theme to {0} and {1}", this.SelectedTheme, this.SelectedAccent);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent(this.SelectedTheme),
                                        ThemeManager.GetAppTheme(this.SelectedAccent));

            if (_Logger.IsInfoEnabled)
                _Logger.Info("Changed theme to {0} and {1}", this.SelectedTheme, this.SelectedAccent);
        }

        #endregion

        #region Methods

        private static void ClickButton([NotNull] IInputElement someButton)
        {
            if (someButton == null)
                throw new ArgumentNullException("someButton");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            someButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private static void OnSelectedAccentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (MainWindow)sender;
            self.ChangeTheme();
        }

        private static void OnSelectedThemeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var self = (MainWindow)sender;
            self.ChangeTheme();
        }

        private async void ExitMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Logger.IsDebugEnabled)
                _Logger.Debug("در حال خروج از برنامه");

            this.Show();
            this.Topmost = true;
            UIHelper.SetTimeout(2000, self => self.Topmost = false, this);

            if (await this.ShowMessageAsync("تایید خروج", "آیا در مورد خروج از برنامه مطمئنید؟", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "بله", NegativeButtonText = "خیر", AnimateHide = false, AnimateShow = false }) != MessageDialogResult.Affirmative)
            {
                if (_Logger.IsInfoEnabled)
                    _Logger.Info("لغو خروج از برنامه");
                return;
            }

            var loginData = await this.ShowLoginAsync("تایید خروج", "لطفا رمز خروج از برنامه را وارد کنید", new LoginDialogSettings { AffirmativeButtonText = "خروج", NegativeButtonText = "لغو", ColorScheme = MetroDialogColorScheme.Inverted, InitialUsername = "exit", PasswordWatermark = "رمز خروج", UsernameWatermark = "دستور", AnimateShow = false, AnimateHide = false });
            var passHash = Crypto.GetSignature(loginData.Password, loginData.Password);
            if (passHash != @"nKdw553HItLjJNzIeE+quw==" /*payapaya*/)
            {
                if (_Logger.IsInfoEnabled)
                    _Logger.Info("لغو خروج از برنامه");

                //if (loginData.Username.Equals(@"config", StringComparison.OrdinalIgnoreCase)
                //    || loginData.Username.Equals(@"c", StringComparison.OrdinalIgnoreCase))
                //{
                //    switch (loginData.Password)
                //    {
                //        case @"updaters":
                //        case @"updates":
                //        case @"u":
                //            {
                //                var updatersWin = new UpdatesWindow();
                //                updatersWin.Show();
                //            }
                //            break;

                //        default:
                //            if (_Logger.IsDebugEnabled)
                //                _Logger.Debug("Unknown configuration: {0}", loginData.Password);
                //            break;
                //    }
                //}
                //else if (loginData.Username.Equals(@"updater", StringComparison.OrdinalIgnoreCase)
                //    || loginData.Username.Equals(@"u", StringComparison.OrdinalIgnoreCase))
                //{
                //    Uri uri;
                //    if (Uri.TryCreate(loginData.Password, UriKind.Absolute, out uri))
                //    {
                //        App.ClientUpdaterFactory.CreateUpdater(uri.GetComponents(UriComponents.HostAndPort | UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.SafeUnescaped));

                //        if (_Logger.IsInfoEnabled)
                //            _Logger.Info("Created updater to {0}", uri);

                //        await App.ClientUpdaterFactory.TriggerAsync(this._CancellationTokenSource);
                //    }
                //    else
                //    {
                //        if (_Logger.IsErrorEnabled)
                //            _Logger.Error("Cannot create updater to {0}", loginData.Password);
                //    }
                //}
                /*else*/ if (loginData.Username.Equals(@"help", StringComparison.OrdinalIgnoreCase))
                {
                    await this.ShowMessageAsync("HELP", @"help\r\nconfig updaters\r\nupdater <uri>\r\n");
                }
                else
                {
#if !DEBUG
                    await this.ShowMessageAsync("خطا", @"رمز خروج اشتباه است");
#else
                    _Logger.Debug(() => "Debug exiting");
#endif
                }

#if !DEBUG
                return;
#endif
            }
            else
            {
                if (_Logger.IsInfoEnabled)
                    _Logger.Info("تایید خروج از برنامه");

                this._FormLoaded = false;

                try
                {
                    Application.Current.Shutdown(0);
                }
                catch (Exception exp)
                {
                    if (_Logger.IsWarnEnabled)
                        _Logger.Warn(exp, "خطا حین خروج از برنامه");

                    try
                    {
                        Environment.Exit(exp.GetBaseException().GetHashCode());
                    }
                    catch (Exception exp2)
                    {
                        if (_Logger.IsWarnEnabled)
                            _Logger.Warn(exp2, "خطا حین خروج از برنامه");

                        Environment.FailFast("خطا حین خروج از برنامه", exp2);
                    }
                }
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ClickButton(this.ExitMenuButton);
        }

        private void HideMenuButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void LogsTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender == null || e == null)
                return;

            var textBox = sender as FrameworkElement;
            if (textBox == null)
                return;

            switch (e.Key)
            {
                case Key.F4:
                    textBox.FlowDirection = textBox.FlowDirection == FlowDirection.RightToLeft ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
                    break;
                case Key.F1:
                    this.ShowAboutWindow();
                    break;
            }
        }

        private void ShowAboutWindow()
        {
            var win = new AboutWindow { Owner = this };
            win.ShowDialog();
        }

        private void ShowAboutWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.ShowAboutWindow();
        }


        private void ShowWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }


        /// <summary>
        ///     Handles the Closing event of the TheMainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void TheMainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
            _Logger.Debug("مخفی کردن پنجره‌ی اصلی برنامه");
        }

        private async void TheMainWindow_Initialized(object sender, EventArgs e)
        {
            if (_Logger.IsTraceEnabled)
            {
                _Logger.Trace("آماده‌سازی پنجره‌ی اصلی برنامه");
            }


            //await TaskEx.Run(() =>
            //{
            //    this.Dispatcher.Invoke(new Action(delegate
            //    {
            //        if (_Logger.IsDebugEnabled)
            //        {
            //            _Logger.Debug("در حال آماده‌سازی پنجره‌ی ویراستار");
            //        }

            //        var w = new PayaEditorWindow { WindowState = WindowState.Minimized };
            //        w.Close();

            //        if (_Logger.IsDebugEnabled)
            //        {
            //            _Logger.Debug("آماده‌سازی پنجره‌ی ویراستار انجام شد.");
            //        }
            //    }));
            //});
        }

        private void TheMainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this._FormLoaded)
            {
                return;
            }

            using (var k = Registry.CurrentUser.CreateSubKey(RegstryKeyLocation))
            {
                if (k != null)
                {
                    k.SetValue("MainWinVisible", this.IsVisible);

                    if (_Logger.IsDebugEnabled)
                    {
                        _Logger.Debug("وضعیت {0} پنجره اصلی برنامه ثبت گردید.", this.Visibility);
                    }
                }
            }

            if (!this.IsVisible)
                this.TrayIcon.ShowBalloonTip("ویرایشگر پایا", "ویرایشگر پایا مخفی شد. برای نمایش دو بار کلیک کنید.", BalloonIcon.Info);
        }

        private void TheMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Logger.IsTraceEnabled)
            {
                _Logger.Trace("در حال بارگذاری پنجره‌ی اصلی");
            }

            if (App.IsStartup)
            {
                this.Visibility = Visibility.Hidden;
            }
            else
            {
                try
                {
                    using (var k = Registry.CurrentUser.OpenSubKey(RegstryKeyLocation, false))
                    {
                        if (k != null)
                        {
                            bool v = Convert.ToBoolean(k.GetValue("MainWinVisible", false));

                            if (!v)
                            {
                                this.Visibility = Visibility.Hidden;

                                if (_Logger.IsDebugEnabled)
                                {
                                    _Logger.Debug("پنجره‌ی اصلی برنامه مخفی گردید.");
                                }
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    _Logger.Warn(exp, "خطا حین خواندن وضعیت پنجره‌ی اصلی");
                    this.Visibility = Visibility.Hidden;
                }
            }

            this._FormLoaded = true;

            if (_Logger.IsTraceEnabled)
            {
                _Logger.Trace("پنجره‌ی اصلی بارگذاری شد");
            }

            if (App.IsUpdateStartup)
            {
                this.TrayIcon.ShowBalloonTip("ویرایشگر پایا", "سرویس ویرایشگر پایا به روز شد.", BalloonIcon.Info);
            }
        }

        private void TrayIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (_Logger.IsDebugEnabled)
            {
                _Logger.Debug("نمایش پنجره‌ی اصلی برنامه");
            }

            this.Visibility = Visibility.Visible;
        }

        #endregion

        private async void CheckForUpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource cts;

            lock (this._CancellationTokenSourceSyncObj)
            {
                if (this._CancellationTokenSource != null)
                    return;
                this._CancellationTokenSource = cts = new CancellationTokenSource();
            }

            try
            {
                await App.CheckForUpdatesAsync(cts);
            }
            finally
            {
                lock (this._CancellationTokenSourceSyncObj)
                {
                    this._CancellationTokenSource = null;
                }
            }
        }
        private void Test_CheckBox_Visible_Click(object sender, RoutedEventArgs e)
        {
            Test = true;
        }

        private void OpenMessagesShowButton_Click(object sender, RoutedEventArgs e)
        {
            this.ShowOpenedWindows();
        }

        private void ShowOpenMessagesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.ShowOpenedWindows();
        }

        private void ShowOpenedWindows()
        {
            this.Topmost = false;
            var w = this.OpenMessages;
            w.Show();
            w.Activate();
        }


        public bool EnableAutoUpdate
        {
            get { return (bool)GetValue(EnableAutoUpdateProperty); }
            set { SetValue(EnableAutoUpdateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAutoUpdate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAutoUpdateProperty =
            DependencyProperty.Register("EnableAutoUpdate", typeof(bool), typeof(MainWindow), new PropertyMetadata(true, OnEnableAutoUpdateChanged));

        private static void OnEnableAutoUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                using (var key = GetEnableAutoUpdateKey())
                {
                    if (key != null)
                    {
                        key.SetValue("EnableAutoUpdate", e.NewValue);
                        key.Close();
                    }
                }

                //foreach (var u in App.ClientUpdaterFactory.Updaters)
                //    u.EnableAutoUpdate = (bool)e.NewValue;
            }
            catch (Exception exp)
            {
                if (_Logger.IsWarnEnabled)
                {
                    _Logger.Warn(exp, "Error while saving EnableAutoUpdate into the registry");
                }
            }
        }

        private static RegistryKey GetEnableAutoUpdateKey()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Paya\Pendar\Editor", true);
            if (key == null)
            {
                Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("Paya").CreateSubKey("Pendar").CreateSubKey("Editor").Close();
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Paya\Pendar\Editor", true);
            }
            return key;
        }
    }
}
