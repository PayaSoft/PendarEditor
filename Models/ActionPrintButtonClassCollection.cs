using System;

namespace Paya.Automation.Editor.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public sealed class ActionPrintButtonClassCollection : ObservableCollection<ActionPrintButtonClass>
    {
        #region Constructors and Destructors

        public ActionPrintButtonClassCollection()
        {
        }

        public ActionPrintButtonClassCollection(IEnumerable<ActionPrintButtonClass> items)
            : base(items)
        {
        }

        #endregion
    }
}