using System;

namespace Paya.Automation.Editor.Models
{
    using JetBrains.Annotations;

    [Serializable]
    public sealed class SendFaxData : BaseInpc
    {
        #region Constants

        /// <summary>
        ///     The <see cref="FaxNumber" /> property's name.
        /// </summary>
        public const string FaxNumberPropertyName = "FaxNumber";

        /// <summary>
        ///     The <see cref="Item" /> property's name.
        /// </summary>
        public const string ItemPropertyName = "Item";

        /// <summary>
        ///     The <see cref="PersonId" /> property's name.
        /// </summary>
        public const string PersonIdPropertyName = "PersonId";

        /// <summary>
        ///     The <see cref="ReceiverName" /> property's name.
        /// </summary>
        public const string ReceiverNamePropertyName = "ReceiverName";

        #endregion

        #region Fields

        private readonly Guid _Id = Guid.NewGuid();

        private string _FaxNumber;


        private DataGridItem _Item;

        private long _PersonId;

        private string _ReceiverName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SendFaxData" /> class.
        /// </summary>
        internal SendFaxData()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="FaxNumber" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public string FaxNumber
        {
            get { return this._FaxNumber; }

            set
            {
                if (this._FaxNumber == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._FaxNumber = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        public Guid Id
        {
            get { return this._Id; }
        }

        /// <summary>
        ///     Gets the <see cref="Item" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [CanBeNull]
        public DataGridItem Item
        {
            get { return this._Item; }

            set
            {
                if (this._Item == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Item = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="PersonId" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public long PersonId
        {
            get { return this._PersonId; }

            set
            {
                if (this._PersonId == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._PersonId = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="ReceiverName" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public string ReceiverName
        {
            get { return this._ReceiverName; }

            set
            {
                if (this._ReceiverName == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._ReceiverName = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
