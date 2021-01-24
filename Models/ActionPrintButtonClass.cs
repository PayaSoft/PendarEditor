using System;

namespace Paya.Automation.Editor.Models
{
    using System.Collections.Generic;
    using System.Windows;

    public sealed class ActionPrintButtonClass : DependencyObject
    {
        #region Constants

        /// <summary>
        ///     The <see cref="FaxAction" /> dependency property's name.
        /// </summary>
        public const string FaxActionPropertyName = "FaxAction";

        /// <summary>
        ///     The <see cref="WithCopy" /> dependency property's name.
        /// </summary>
        public const string WithCopyPropertyName = "WithCopy";

        /// <summary>
        ///     The <see cref="WithHeder" /> dependency property's name.
        /// </summary>
        public const string WithHederPropertyName = "WithHeder";

        /// <summary>
        ///     The <see cref="WithSign" /> dependency property's name.
        /// </summary>
        public const string WithSignPropertyName = "WithSign";

        #endregion

        #region Static Fields

        /// <summary>
        ///     Identifies the <see cref="FaxAction" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty FaxActionProperty = DependencyProperty.Register(
                                                                                                  FaxActionPropertyName,
                                                                                                  typeof (bool),
                                                                                                  typeof (ActionPrintButtonClass),
                                                                                                  new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="WithCopy" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty WithCopyProperty = DependencyProperty.Register(
                                                                                                 WithCopyPropertyName,
                                                                                                 typeof (bool),
                                                                                                 typeof (ActionPrintButtonClass),
                                                                                                 new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="WithHeder" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty WithHederProperty = DependencyProperty.Register(
                                                                                                  WithHederPropertyName,
                                                                                                  typeof (bool),
                                                                                                  typeof (ActionPrintButtonClass),
                                                                                                  new PropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="WithSign" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty WithSignProperty = DependencyProperty.Register(
                                                                                                 WithSignPropertyName,
                                                                                                 typeof (bool),
                                                                                                 typeof (ActionPrintButtonClass),
                                                                                                 new PropertyMetadata(false));

        #endregion

        #region Constructors and Destructors

        public ActionPrintButtonClass()
        {
            this.WithHeder = false;
            this.WithSign = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the value of the <see cref="FaxAction" />
        ///     property. This is a dependency property.
        /// </summary>
        public bool FaxAction
        {
            get { return (bool) this.GetValue(FaxActionProperty); }
            set { this.SetValue(FaxActionProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="WithCopy" />
        ///     property. This is a dependency property.
        /// </summary>
        public bool WithCopy
        {
            get { return (bool) this.GetValue(WithCopyProperty); }
            set { this.SetValue(WithCopyProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="WithHeder" />
        ///     property. This is a dependency property.
        /// </summary>
        public bool WithHeder
        {
            get { return (bool) this.GetValue(WithHederProperty); }
            set { this.SetValue(WithHederProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the value of the <see cref="WithSign" />
        ///     property. This is a dependency property.
        /// </summary>
        public bool WithSign
        {
            get { return (bool) this.GetValue(WithSignProperty); }
            set { this.SetValue(WithSignProperty, value); }
        }

        #endregion
    }
}
