using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Models
{
    [Serializable]
    public sealed class AutmationCenterInfo : DataGridItem
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutmationCenterInfo"/> class.
        /// </summary>
        public AutmationCenterInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutmationCenterInfo" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="ref">The reference.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="isCurrent">if set to <c>true</c> [is current].</param>
        /// <param name="isSuccessful">if set to <c>true</c> [is successful].</param>
        /// <param name="requestDate">The request date.</param>
        /// <param name="sentDate">The sent date.</param>
        /// <param name="senderUserDisplayName">Display name of the sender user.</param>
        /// <param name="lastError">The last error.</param>
        /// <param name="logsLength">Length of the logs.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        public AutmationCenterInfo(long id, string @ref, string displayName, bool isCurrent, bool? isSuccessful, DateTime? requestDate, DateTime? sentDate, string senderUserDisplayName, string lastError, int? logsLength, bool isSelected = false)
            : base(id, @ref, displayName, isSelected)
        {
            this._IsCurrent = isCurrent;
            this._IsSuccessful = isSuccessful;
            this._RequestDate = requestDate;
            this._SentDate = sentDate;
            this._SenderUserDisplayName = senderUserDisplayName;
            this._LastError = lastError;
            this._LogsLength = logsLength;
        }



        #endregion

        /// <summary>
            /// The <see cref="IsCurrent" /> property's name.
            /// </summary>
        public const string IsCurrentPropertyName = "IsCurrent";

        private bool _IsCurrent;

        /// <summary>
        /// Gets the <see cref="IsCurrent" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                return this._IsCurrent;
            }

            set
            {
                if (this._IsCurrent == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsCurrent = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="IsSuccessful" /> property's name.
            /// </summary>
        public const string IsSuccessfulPropertyName = "IsSuccessful";

        private bool? _IsSuccessful;

        /// <summary>
        /// Gets the <see cref="IsSuccessful" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public bool? IsSuccessful
        {
            get
            {
                return this._IsSuccessful;
            }

            set
            {
                if (this._IsSuccessful == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsSuccessful = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="RequestDate" /> property's name.
            /// </summary>
        public const string RequestDatePropertyName = "RequestDate";

        private DateTime? _RequestDate;

        /// <summary>
        /// Gets the <see cref="RequestDate" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public DateTime? RequestDate
        {
            get
            {
                return this._RequestDate;
            }

            set
            {
                if (this._RequestDate == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._RequestDate = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="SentDate" /> property's name.
            /// </summary>
        public const string SentDatePropertyName = "SentDate";

        private DateTime? _SentDate;

        /// <summary>
        /// Gets the <see cref="SentDate" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public DateTime? SentDate
        {
            get
            {
                return this._SentDate;
            }

            set
            {
                if (this._SentDate == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._SentDate = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="SenderUserDisplayName" /> property's name.
            /// </summary>
        public const string SenderUserDisplayNamePropertyName = "SenderUserDisplayName";

        private string _SenderUserDisplayName;

        /// <summary>
        /// Gets the <see cref="SenderUserDisplayName" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public string SenderUserDisplayName
        {
            get
            {
                return this._SenderUserDisplayName;
            }

            set
            {
                if (this._SenderUserDisplayName == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._SenderUserDisplayName = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }
        /// <summary>
            /// The <see cref="LastError" /> property's name.
            /// </summary>
        public const string LastErrorPropertyName = "LastError";

        private string _LastError;

        /// <summary>
        /// Gets the <see cref="LastError" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public string LastError
        {
            get
            {
                return this._LastError;
            }

            set
            {
                if (this._LastError == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._LastError = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="LogsLength" /> property's name.
            /// </summary>
        public const string LogsLengthPropertyName = "LogsLength";

        private int? _LogsLength;

        /// <summary>
        /// Gets the <see cref="LogsLength" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public int? LogsLength
        {
            get
            {
                return this._LogsLength;
            }

            set
            {
                if (this._LogsLength == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._LogsLength = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="IsPinging" /> property's name.
            /// </summary>
        public const string IsPingingPropertyName = "IsPinging";

        private bool _IsPinging;

        /// <summary>
        /// Gets the <see cref="IsPinging" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public bool IsPinging
        {
            get
            {
                return this._IsPinging;
            }

            set
            {
                if (this._IsPinging == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsPinging = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }


        /// <summary>
            /// The <see cref="IsSendingMessage" /> property's name.
            /// </summary>
        public const string IsSendingMessagePropertyName = "IsSendingMessage";

        private bool _IsSendingMessage;

        /// <summary>
        /// Gets the <see cref="IsSendingMessage" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public bool IsSendingMessage
        {
            get
            {
                return this._IsSendingMessage;
            }

            set
            {
                if (this._IsSendingMessage == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsSendingMessage = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
            /// The <see cref="IsReceivingWorkflow" /> property's name.
            /// </summary>
        public const string IsReceivingWorkflowPropertyName = "IsReceivingWorkflow";

        private bool _IsReceivingWorkflow;

        /// <summary>
        /// Gets the <see cref="IsReceivingWorkflow" /> property.
        /// <para>Changes to that property's value raise the PropertyChanged event.</para> 
        /// </summary>
        public bool IsReceivingWorkflow
        {
            get
            {
                return this._IsReceivingWorkflow;
            }

            set
            {
                if (this._IsReceivingWorkflow == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsReceivingWorkflow = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        
    }
}
