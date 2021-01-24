using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using Paya.Automation.Editor.Models;

namespace Paya.Automation.Editor
{
    /// <summary>
    /// Interaction logic for OpenMessages.xaml
    /// </summary>
    [System.CLSCompliant(false)]
    public partial class OpenMessages
    {
        public OpenMessages()
        {
            InitializeComponent();
        }

        private async void CloseDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            var s = sender as FrameworkElement;
            if (s == null)
                return;
            var msgInfo = s.Tag as MessageInfo;
            if (msgInfo == null)
                return;

            var r = await this.ShowMessageAsync("تایید بستن نامه", "با بسته‌شدن این نامه، تمامی تغییرات داده شده لغو خواهد شد. آیا مطمئنید؟", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "بله", NegativeButtonText = "خیر", ColorScheme = MetroDialogColorScheme.Inverted });

            if (r == MessageDialogResult.Negative)
                return;

            msgInfo.CloseRequested = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
