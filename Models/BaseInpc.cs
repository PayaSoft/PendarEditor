namespace Paya.Automation.Editor.Models
{
	using System;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using System.Runtime.Serialization;
	using JetBrains.Annotations;

    /// <summary>
	///     Specifies the <see cref="BaseInpc" /> abstract class which provides the <see cref="INotifyPropertyChanged" /> and
	///     <see cref="INotifyPropertyChanging" /> events.
	/// </summary>
	[Serializable]
	[DataContract(Namespace = Internal.DefaultNamespace)]
	public abstract class BaseInpc : INotifyPropertyChanged, INotifyPropertyChanging
	{
		#region Public Events

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///     Occurs when a property value is changing.
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Methods

        protected void OnPropertyChanged([NotNull]string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///     Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
                OnPropertyChanged(propertyName);
        }

		/// <summary>
		///     Raises the property changing event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		protected virtual void RaisePropertyChanging([CallerMemberName]string propertyName = null)
		{
			var handler = this.PropertyChanging;
			if (handler != null)
			{
				handler(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		#endregion
	}
}