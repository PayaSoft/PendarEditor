using System;

namespace Paya.Automation.Editor.Models
{
    using JetBrains.Annotations;

    [Serializable]
    public sealed class SendEmailData : BaseInpc
    {
        #region Constants

        /// <summary>
        ///     The <see cref="EmailAddress" /> property's name.
        /// </summary>
        public const string EmailAddressPropertyName = "EmailAddress";

        /// <summary>
        ///     The <see cref="EmailBccCc" /> property's name.
        /// </summary>
        public const string EmailBccCcPropertyName = "EmailBccCc";

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

        private string _EmailAddress;

        private EmailBccCc _emailBccCc;

        private DataGridItem _Item;

        private long _PersonId;

        private string _ReceiverName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SendEmailData" /> class.
        /// </summary>
        internal SendEmailData()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="EmailAddress" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public string EmailAddress
        {
            get { return this._EmailAddress; }

            set
            {
                if (this._EmailAddress == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._EmailAddress = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="EmailBccCc" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public EmailBccCc EmailBccCc
        {
            get { return this._emailBccCc; }

            set
            {
                if (this._emailBccCc == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._emailBccCc = value;

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
