using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paya.Automation.Editor.Models
{
    using System.Collections.ObjectModel;
    using JetBrains.Annotations;

    [Serializable]
    public sealed class AutomationCenterWorkflowModel : BaseInpc
    {
        #region Constants

        /// <summary>
        ///     The <see cref="Center" /> property's name.
        /// </summary>
        public const string CenterPropertyName = "Center";

        /// <summary>
        ///     The <see cref="Workflow" /> property's name.
        /// </summary>
        public const string WorkflowPropertyName = "Workflow";

        #endregion

        #region Fields

        private AutmationCenterInfo _Center;

        private ObservableCollection<WorkflowNodeModel> _Workflow;

        #endregion

        public AutomationCenterWorkflowModel()
        {
            
        }

        public AutomationCenterWorkflowModel([NotNull] AutmationCenterInfo center, [NotNull] ObservableCollection<WorkflowNodeModel> workflow)
        {
            if (center == null) throw new ArgumentNullException("center");
            if (workflow == null) throw new ArgumentNullException("workflow");
            this._Center = center;
            this._Workflow = workflow;
        }

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="Center" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public AutmationCenterInfo Center
        {
            get { return this._Center; }

            set
            {
                if (this._Center == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Center = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Gets the <see cref="Workflow" /> property.
        ///     <para>Changes to that property's value raise the PropertyChanged event.</para>
        /// </summary>
        public ObservableCollection<WorkflowNodeModel> Workflow
        {
            get { return this._Workflow; }

            set
            {
                if (this._Workflow == value)
                {
                    return;
                }

                this.RaisePropertyChanging();

                this._Workflow = value;

                // Update bindings, no broadcast
                this.RaisePropertyChanged();
            }
        }

        #endregion
    }
}
