using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Paya.Automation.Editor.Models
{
    [Serializable]
    public sealed class MessageInfo : BaseInpc
    {
        [NotNull]
        private readonly MessageSessionData _SessionData;

        [NotNull]
        private readonly string _DisplayName;

        [NotNull]
        public MessageSessionData SessionData
        {
            get
            {
                return _SessionData;
            }
        }

        [NotNull]
        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
        }

        /// <summary>
        /// The <see cref="CloseRequested" /> property's name.
        /// </summary>
        public const string CloseRequestedPropertyName = "CloseRequested";

        private bool _myProperty = false;

        /// <summary>
        /// Gets the <see cref="CloseRequested" /> property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool CloseRequested
        {
            get
            {
                return _myProperty;
            }

            internal set
            {
                if (_myProperty == value)
                {
                    return;
                }

                _myProperty = value;

                // Update bindings, no broadcast
                RaisePropertyChanged();
            }
        }

        public MessageInfo([NotNull]MessageSessionData sessionData, [NotNull]string displayName)
        {
            if (sessionData == null)
                throw new ArgumentNullException("sessionData");
            if (displayName == null)
                throw new ArgumentNullException("displayName");
            System.Diagnostics.Contracts.Contract.EndContractBlock();

            this._SessionData = sessionData;
            this._DisplayName = displayName;
        }
    }
}
