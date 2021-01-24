using System;

namespace Paya.Automation.Editor.Models
{
    using System.ComponentModel;

    public interface IDataGridItem : IEquatable<IDataGridItem>, INotifyPropertyChanged
    {
        #region Public Properties

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        long Id { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        bool IsSelected { get; set; }

        #endregion
    }
}
