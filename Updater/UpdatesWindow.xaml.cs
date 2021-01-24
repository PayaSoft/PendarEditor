using System;

namespace Paya.Automation.Editor.Updater
{
    using System.Windows;
    using MahApps.Metro.Controls.Dialogs;
    using NLog;

    /// <summary>
    ///     Interaction logic for UpdatesWindow.xaml
    /// </summary>
    [CLSCompliant(false)]
    public partial class UpdatesWindow
    {
        #region Static Fields

        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        public UpdatesWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await this.ShowMessageAsync("حذف آدرس به روز رسانی", "آیا مطمئن به حذف این آدرس به روز رسانی هستید؟", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "بله", NegativeButtonText = "خیر", AnimateShow = false, AnimateHide = false }) == MessageDialogResult.Negative)
                    return;

                var updater = ((FrameworkElement)sender).Tag as PayaClientUpdater;

                if (updater == null)
                    return;

                lock (this)
                {
                    App.ClientUpdaterFactory.DeleteUpdater(updater.BaseUrl);
                }
            }
            catch (Exception exp)
            {
                this.ShowException(exp);
            }
        }

        private async void ShowException(Exception exp, string logMessage = null)
        {
            if (_Logger.IsErrorEnabled)
                _Logger.Error(exp, logMessage ?? exp.GetBaseException().Message);


            await this.ShowMessageAsync(exp.GetBaseException().GetType().Name, exp.Message, settings: new MetroDialogSettings { AffirmativeButtonText = "بستن" });
        }

        #endregion
    }
}
