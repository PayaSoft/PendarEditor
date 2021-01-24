namespace Paya.Automation.Editor.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;


    /// <summary>
    ///     Specifies the <see cref="EceData" /> sealed class.
    /// </summary>
    [DataContract(Namespace = Internal.DefaultNamespace)]
    [Serializable]
    public sealed class EceData : BaseInpc
    {
        #region Constants

        /// <summary>
        ///     The <see cref="Classification" /> property's name.
        /// </summary>
        public const string ClassificationPropertyName = "Classification";

        /// <summary>
        ///     The <see cref="Priority" /> property's name.
        /// </summary>
        public const string PriorityPropertyName = "Priority";


        /// <summary>
        ///     The <see cref="Receiver" /> property's name.
        /// </summary>
        public const string ReceiverPropertyName = "Receiver";

        /// <summary>
        ///     The <see cref="Sender" /> property's name.
        /// </summary>
        public const string SenderPropertyName = "Sender";

        /// <summary>
        ///     The <see cref="SendType" /> property's name.
        /// </summary>
        public const string SendTypePropertyName = "SendType";

        #endregion

        #region Fields

        private EceClassification _Classification;

        private EcePriority _Priority;

        private EceTargetData _Receiver;

        private EceTargetData _Sender;

        private EceSendType _SendType;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="Classification" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public EceClassification Classification
        {
            get { return this._Classification; }

            set
            {
                if (this._Classification == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Classification = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Priority" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public EcePriority Priority
        {
            get { return this._Priority; }

            set
            {
                if (this._Priority == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Priority = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Receiver" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public EceTargetData Receiver
        {
            get { return this._Receiver; }

            set
            {
                if (this._Receiver == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Receiver = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Sender" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public EceTargetData Sender
        {
            get { return this._Sender; }

            set
            {
                if (this._Sender == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Sender = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="SendType" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public EceSendType SendType
        {
            get { return this._SendType; }

            set
            {
                if (this._SendType == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._SendType = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        #endregion

    }

    /// <summary>
    ///     Specifies the <see cref="EceTargetData" /> sealed class.
    /// </summary>
    [DataContract(Namespace = Internal.DefaultNamespace)]
    [Serializable]
    public sealed class EceTargetData : BaseInpc
    {
        #region Constants

        /// <summary>
        ///     The <see cref="Code" /> property's name.
        /// </summary>
        public const string CodePropertyName = "Code";

        /// <summary>
        ///     The <see cref="Department" /> property's name.
        /// </summary>
        public const string DepartmentPropertyName = "Department";

        /// <summary>
        ///     The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        /// <summary>
        ///     The <see cref="Organization" /> property's name.
        /// </summary>
        public const string OrganizationPropertyName = "Organization";

        /// <summary>
        ///     The <see cref="Position" /> property's name.
        /// </summary>
        public const string PositionPropertyName = "Position";

        #endregion

        #region Fields

        private string _Code;

        private string _Department;

        private string _Name;

        private string _Organization;

        private string _Position;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="Code" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public string Code
        {
            get { return this._Code; }

            set
            {
                if (this._Code == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Code = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Department" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public string Department
        {
            get { return this._Department; }

            set
            {
                if (this._Department == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Department = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Name" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return this._Name; }

            set
            {
                if (this._Name == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Name = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Organization" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public string Organization
        {
            get { return this._Organization; }

            set
            {
                if (this._Organization == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Organization = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Position" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        [DataMember]
        public string Position
        {
            get { return this._Position; }

            set
            {
                if (this._Position == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Position = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        #endregion
    }

    #region Enums

    [Serializable]
    public enum EceClassification
    {
        [Description("عادی")]
        Normal,
        [Description("محرمانه")]
        Secret,
        [Description("سری")]
        Occult
    }

    [Serializable]
    public enum EcePriority
    {
        [Description("عادی")]
        Normal,
        [Description("فوری")]
        Immediate,
        [Description("آنی")]
        Instantaneous
    }

    [Serializable]
    public enum EceSendType
    {
        [Description("اصل")]
        Origin,
        [Description("کپی")]
        Copy,
        [Description("ارجاع")]
        Refer
    }

    #endregion
}
