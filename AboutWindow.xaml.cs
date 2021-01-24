using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    ///     Interaction logic for AboutWindow.xaml
    /// </summary>
    [CLSCompliant(false)]
    public partial class AboutWindow
    {
        #region Constructors and Destructors

        public AboutWindow()
        {
            this.InitializeComponent();
        }

        #endregion

        /// <summary>
            /// The <see cref="ProductVersion" /> dependency property's name.
            /// </summary>
        public const string ProductVersionPropertyName = "ProductVersion";

        /// <summary>
        /// Gets or sets the value of the <see cref="ProductVersion" />
        /// property. This is a dependency property.
        /// </summary>
        public Version ProductVersion
        {
            get
            {
                return (Version)GetValue(ProductVersionProperty);
            }
            set
            {
                SetValue(ProductVersionProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ProductVersion" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProductVersionProperty = DependencyProperty.Register(
            ProductVersionPropertyName,
            typeof(Version),
            typeof(AboutWindow),
            new UIPropertyMetadata(null));

        /// <summary>
            /// The <see cref="AssemblyVersion" /> dependency property's name.
            /// </summary>
        public const string AssemblyVersionPropertyName = "AssemblyVersion";

        /// <summary>
        /// Gets or sets the value of the <see cref="AssemblyVersion" />
        /// property. This is a dependency property.
        /// </summary>
        public Version AssemblyVersion
        {
            get
            {
                return (Version)GetValue(AssemblyVersionProperty);
            }
            set
            {
                SetValue(AssemblyVersionProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="AssemblyVersion" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AssemblyVersionProperty = DependencyProperty.Register(
            AssemblyVersionPropertyName,
            typeof(Version),
            typeof(AboutWindow),
            new UIPropertyMetadata(null));

        private void TheAboutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string versionStr = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            Version v;
            if (Version.TryParse(versionStr, out v))
                this.ProductVersion = v;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
