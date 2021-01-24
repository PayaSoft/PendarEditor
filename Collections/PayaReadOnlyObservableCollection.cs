namespace Paya.Automation.Editor.Collections
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;

	/// <summary>
	/// 	Represents a readonly dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
	/// </summary>
	/// <typeparam name="T" />
	public class PayaReadOnlyObservableCollection<T> : ReadOnlyObservableCollection<T>, IObservableCollection<T>
	{
		#region Constructors and Destructors

		/// <summary>
		/// 	Initializes a new instance of the <see cref="System.Collections.ObjectModel.ObservableCollection{T}" /> class that contains elements copied from the specified collection.
		/// </summary>
		/// <param name="collection"> collection: The collection from which the elements are copied. </param>
		/// <exception cref="ArgumentNullException">The collection parameter cannot be null.</exception>
		public PayaReadOnlyObservableCollection(ObservableCollection<T> collection)
			: base(collection) {}

        /// <summary>
        /// 	Initializes a new instance of the <see cref="PayaObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="items"> The items. </param>
        public PayaReadOnlyObservableCollection(IEnumerable<T> items)
			: base(new PayaObservableCollection<T>(items)) {}

        /// <summary>
        /// 	Initializes a new instance of the <see cref="PayaObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="items"> The items. </param>
        public PayaReadOnlyObservableCollection(params T[] items)
			: base(new PayaObservableCollection<T>(items)) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// 	Adds the elements of the specified collection to the end of the <see cref="System.Collections.ObjectModel.ObservableCollection{T}" /> with a single <see cref="ReadOnlyObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        /// <param name="items"> The items. </param>
        public void AddRange(params T[] items)
		{
			if( items == null )
				throw new ArgumentNullException("items");

			this.AddRange(items.AsEnumerable());
		}

        /// <summary>
        /// 	Removes the first occurence of each item in the specified collection from <see cref="System.Collections.ObjectModel.ObservableCollection{T}" /> with a single <see cref="ReadOnlyObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        public void RemoveRange(IEnumerable<T> collection)
		{
			if( collection == null )
				throw new ArgumentNullException("collection");

			foreach( var i in collection )
			{
				this.Items.Remove(i);
			}
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

        /// <summary>
        /// 	Clears the current collection and replaces it with the specified item with a single <see cref="ReadOnlyObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        public void Replace(T item)
		{
			this.ReplaceRange(new[] { item });
		}

        /// <summary>
        /// 	Clears the current collection and replaces it with the specified collection with a single <see cref="ReadOnlyObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        public void ReplaceRange(IEnumerable<T> collection)
		{
			if( collection == null )
				throw new ArgumentNullException("collection");

			this.Items.Clear();
			foreach( var i in collection )
			{
				this.Items.Add(i);
			}
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

        #endregion

        #region Implemented Interfaces

        #region IObservableCollection<T>

        /// <summary>
        /// 	Adds the elements of the specified collection to the end of the <see cref="System.Collections.ObjectModel.ObservableCollection{T}" /> with a single <see cref="ReadOnlyObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public void AddRange(IEnumerable<T> collection)
		{
			if( collection == null )
				throw new ArgumentNullException("collection");

			foreach( var i in collection )
			{
				this.Items.Add(i);
			}
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		#endregion

		#endregion
	}
}