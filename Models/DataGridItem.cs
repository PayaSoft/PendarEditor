using System;

namespace Paya.Automation.Editor.Models
{
    [Serializable]
    public class DataGridItem : BaseInpc, IEquatable<DataGridItem>, IDataGridItem
    {
        #region Constants

        /// <summary>
        ///     The <see cref="DisplayName" /> property's name.
        /// </summary>
        public const string DisplayNamePropertyName = "DisplayName";

        /// <summary>
        ///     The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        /// <summary>
        ///     The <see cref="IsSelected" /> property's name.
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        /// <summary>
        ///     The <see cref="Ref" /> property's name.
        /// </summary>
        public const string RefPropertyName = "Ref";

        /// <summary>
        ///     The <see cref="Tag" /> property's name.
        /// </summary>
        public const string TagPropertyName = "Tag";

        #endregion

        #region Fields

        private string _DisplayName;

        private long _Id;

        private bool _IsSelected;

        private string _Ref;

        private object _Tag;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataGridItem" /> class.
        /// </summary>
        public DataGridItem()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataGridItem" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="ref">The reference.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        public DataGridItem(long id, string @ref, string displayName, bool isSelected = false)
        {
            this._Id = id;
            this._Ref = @ref;
            this._DisplayName = displayName;
            this._IsSelected = isSelected;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="DisplayName" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }

            set
            {
                if (this._DisplayName == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._DisplayName = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Id" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public long Id
        {
            get { return this._Id; }

            set
            {
                if (this._Id == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Id = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="IsSelected" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public bool IsSelected
        {
            get { return this._IsSelected; }

            set
            {
                if (this._IsSelected == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsSelected = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Ref" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public string Ref
        {
            get { return this._Ref; }

            set
            {
                if (this._Ref == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Ref = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Tag" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public object Tag
        {
            get { return this._Tag; }

            set
            {
                if (this._Tag == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Tag = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool operator ==(DataGridItem left, DataGridItem right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;
            return left.Equals(right);
        }

        public static bool operator !=(DataGridItem left, DataGridItem right)
        {
            return !(left == right);
        }

        public bool Equals(DataGridItem other)
        {
            return !ReferenceEquals(other, null) && this.Id == other.Id;
        }

        bool IEquatable<IDataGridItem>.Equals(IDataGridItem other)
        {
            return !ReferenceEquals(other, null) && this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DataGridItem);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #endregion
    }
}
