using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Models
{
    using System.Collections.ObjectModel;

    [Serializable]
    public sealed class WorkflowNodeModel : BaseInpc
    {
        #region Constants

        /// <summary>
        ///     The <see cref="Children" /> property's name.
        /// </summary>
        public const string ChildrenPropertyName = "Children";

        /// <summary>
        ///     The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        /// <summary>
        ///     The <see cref="IsContinued" /> property's name.
        /// </summary>
        public const string IsContinuedPropertyName = "IsContinued";

        /// <summary>
        ///     The <see cref="IsCopy" /> property's name.
        /// </summary>
        public const string IsCopyPropertyName = "IsCopy";

        /// <summary>
        ///     The <see cref="IsCurrent" /> property's name.
        /// </summary>
        public const string IsCurrentPropertyName = "IsCurrent";

        /// <summary>
        ///     The <see cref="IsDummy" /> property's name.
        /// </summary>
        public const string IsDummyPropertyName = "IsDummy";

        /// <summary>
        ///     The <see cref="OpenedAt" /> property's name.
        /// </summary>
        public const string OpenedAtPropertyName = "OpenedAt";

        /// <summary>
        ///     The <see cref="Parent" /> property's name.
        /// </summary>
        public const string ParentPropertyName = "Parent";

        /// <summary>
        ///     The <see cref="ReceivedAt" /> property's name.
        /// </summary>
        public const string ReceivedAtPropertyName = "ReceivedAt";

        /// <summary>
        ///     The <see cref="SentAt" /> property's name.
        /// </summary>
        public const string SentAtPropertyName = "SentAt";

        /// <summary>
        ///     The <see cref="User" /> property's name.
        /// </summary>
        public const string UserPropertyName = "User";

        #endregion

        #region Fields

        private ObservableCollection<WorkflowNodeModel> _Children;

        private int _Id;

        private bool _IsContinued;

        private bool _IsCopy;

        private bool _IsCurrent;

        private bool _IsDummy;

        private DateTime? _openedAt;

        private WorkflowNodeModel _Parent;

        private DateTime? _ReceivedAt;

        private DateTime? _SentAt;

        private string _User;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkflowNodeModel" /> class.
        /// </summary>
        public WorkflowNodeModel()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkflowNodeModel" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <param name="isCurrent">if set to <c>true</c> [is current].</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        /// <param name="isContinued">if set to <c>true</c> [is continued].</param>
        /// <param name="receivedAt">The received at.</param>
        /// <param name="openedAt">The opend at.</param>
        /// <param name="sentAt">The sent at.</param>
        /// <param name="children">The children.</param>
        public WorkflowNodeModel(int id, string user, bool isCurrent, bool isCopy, bool isContinued, DateTime? receivedAt, DateTime? openedAt, DateTime? sentAt, IEnumerable<WorkflowNodeModel> children)
        {
            this._IsDummy = id < 0;
            this._Id = id;
            this._User = user;
            this._IsCurrent = isCurrent;
            this._IsCopy = isCopy;
            this._IsContinued = isContinued;
            this._ReceivedAt = receivedAt;
            this._openedAt = openedAt;
            this._SentAt = sentAt;
            if (children != null)
            {
                this._Children = new ObservableCollection<WorkflowNodeModel>(children);
                foreach (var child in this._Children)
                {
                    child._Parent = this;
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="Children" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public ObservableCollection<WorkflowNodeModel> Children
        {
            get { return this._Children; }

            set
            {
                if (this._Children == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Children = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Id" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public int Id
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
        ///     Gets the <see cref="IsContinued" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public bool IsContinued
        {
            get { return this._IsContinued; }

            set
            {
                if (this._IsContinued == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsContinued = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="IsCopy" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public bool IsCopy
        {
            get { return this._IsCopy; }

            set
            {
                if (this._IsCopy == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsCopy = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="IsCurrent" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public bool IsCurrent
        {
            get { return this._IsCurrent; }

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
        ///     Gets the <see cref="IsDummy" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public bool IsDummy
        {
            get { return this._IsDummy; }

            set
            {
                if (this._IsDummy == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._IsDummy = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="OpenedAt" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public DateTime? OpenedAt
        {
            get { return this._openedAt; }

            set
            {
                if (this._openedAt == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._openedAt = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Parent" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public WorkflowNodeModel Parent
        {
            get { return this._Parent; }

            set
            {
                if (this._Parent == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Parent = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="ReceivedAt" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public DateTime? ReceivedAt
        {
            get { return this._ReceivedAt; }

            set
            {
                if (this._ReceivedAt == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._ReceivedAt = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="SentAt" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public DateTime? SentAt
        {
            get { return this._SentAt; }

            set
            {
                if (this._SentAt == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._SentAt = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="User" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public string User
        {
            get { return this._User; }

            set
            {
                if (this._User == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._User = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
