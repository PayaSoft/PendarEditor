namespace Paya.Automation.Editor.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// 	Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of collection items.</typeparam>
    public class PayaObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>, IObservableCollection<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// 	Initializes a new instance of the <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/> class.
        /// </summary>
        public PayaObservableCollection() {}

        /// <summary>
        /// 	Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection"> collection: The collection from which the elements are copied. </param>
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception>
        public PayaObservableCollection(IEnumerable<T> collection)
            : base(collection) {}

        public PayaObservableCollection(List<T> collection)
            : base(collection)
        { }

        /// <summary>
        /// 	Initializes a new instance of the <see cref="PayaObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="items"> The items. </param>
        public PayaObservableCollection(params T[] items)
            : base(items) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// 	Adds the elements of the specified collection to the end of the ObservableCollection&lt;T&gt; with a single <see cref="ReadOnlyObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        /// <param name="items"> The items. </param>
        public void AddRange(params T[] items)
        {
            if( items == null )
                throw new ArgumentNullException("items");

            this.AddRange(items.AsEnumerable());
        }

        /// <summary>
        /// 	Removes the first occurence of each item in the specified collection from ObservableCollection(Of T) with a single CollectionChanged event.
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

		public void RemoveWhere(Func<T, bool> predicate)
		{
			foreach (var i in this.Items.Where(predicate).ToArray())
			{
				this.Items.Remove(i);
			}

			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public void Swap(int index1, int index2)
		{
			if (index1 < 0 || index1 >= this.Count)
				throw new ArgumentOutOfRangeException("index1");
			if (index2 < 0 || index2 >= this.Count)
				throw new ArgumentOutOfRangeException("index2");

			var tmp = this.Items[index1];
			this.Items[index1] = this.Items[index2];
			this.Items[index2] = tmp;
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

        /// <summary>
        /// 	Clears the current collection and replaces it with the specified item with a single CollectionChanged event.
        /// </summary>
        public void Replace(T item)
        {
            this.ReplaceRange(new[] { item });
        }

        /// <summary>
        /// 	Clears the current collection and replaces it with the specified collection with a single CollectionChanged event.
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
        /// 	Adds the elements of the specified collection to the end of the ObservableCollection(Of T) with a single CollectionChanged event.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException" />
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