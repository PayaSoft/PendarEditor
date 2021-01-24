namespace Paya.Automation.Editor.StyleSelector
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Paya.Automation.Editor.Models;

    public sealed class AutomationCenterRowStyleSelector : StyleSelector
    {
        #region Public Properties

        public Style FailureStyle { get; set; }

        public Style SuccessStyle { get; set; }

        public Style PingingStyle { get; set; }

        #endregion

        #region Public Methods and Operators

        public override Style SelectStyle(object obj, DependencyObject container)
        {
            var item = obj as AutmationCenterInfo;
            if (item == null)
                return null;

            if (item.IsPinging)
                return this.PingingStyle;

            switch (item.IsSuccessful)
            {
                case true:
                    return this.SuccessStyle;

                case false:
                    return this.FailureStyle;

                case null:
                    return null;

                default:
                    return null;
            }
        }

        #endregion
    }
}
