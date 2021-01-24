using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Behaviors
{
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using JetBrains.Annotations;

    public sealed class DateTextBoxBehavior : Behavior<TextBox>
    {
        #region Methods

        protected override void OnAttached()
        {
            this.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
        }

        private static void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox) sender;

            textBox.Text = GetParsedDateString(textBox.Text) ?? string.Empty;
        }


        [ContractAnnotation("null=>null")]
        private static string GetParsedDateString(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var dt = PersianUtility.ParseCultureDate(text);

            return dt == null ? null : dt.ToCultureString();
        }

        #endregion
    }
}
