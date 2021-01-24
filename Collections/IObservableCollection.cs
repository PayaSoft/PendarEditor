namespace Paya.Automation.Editor.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 	Marker interface for the observable collection.
    /// </summary>
    public interface IObservableCollection : IList, INotifyCollectionChanged, INotifyPropertyChanged {}

    /// <summary>
    /// 	Generic interface for observable collection
    /// </summary>
    /// <typeparam name="T"> The type being used for binding. </typeparam>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
    public interface IObservableCollection<T> : IList<T>, IObservableCollection
    {
        #region Public Methods

        /// <summary>
        /// 	Adds a range of items to the observable collection.
        /// </summary>
        /// <param name="items"> The items to be added to the observable collection. </param>
        void AddRange(IEnumerable<T> items);

        #endregion
    }
}