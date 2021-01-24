using System;

namespace Paya.Automation.Editor.Models
{
    [Serializable]
    public sealed class DisplayNameIdPair : BaseInpc
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

        #endregion

        #region Fields

        private string _DisplayName;

        private long _Id;

        #endregion

        #region Constructors and Destructors

        public DisplayNameIdPair()
        {
        }

        public DisplayNameIdPair(long id, string displayName)
        {
            this._Id = id;
            this._DisplayName = displayName;
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

        #endregion
    }
}
