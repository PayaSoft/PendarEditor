using System;

namespace Paya.Automation.Editor.Data
{
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;

    internal class CollectionViewGroupInternal : CollectionViewGroup
    {
        #region Fields

        // Fields
        private GroupDescription _groupBy;
        private int _version;

        #endregion

        #region Constructors and Destructors

        // Methods
        internal CollectionViewGroupInternal(object name, CollectionViewGroupInternal parent) : base(name)
        {
            this.Parent = parent;
        }

        #endregion

        #region Public Properties

        public override bool IsBottomLevel
        {
            get { return (this._groupBy == null); }
        }

        #endregion

        #region Properties

        // Properties
        [DefaultValue(1)]
        internal int FullCount { get; set; }

        internal GroupDescription GroupBy
        {
            get { return this._groupBy; }
            set
            {
                var isBottomLevel = this.IsBottomLevel;
                if (this._groupBy != null)
                {
                    ((INotifyPropertyChanged) this._groupBy).PropertyChanged -= this.OnGroupByChanged;
                }
                this._groupBy = value;
                if (this._groupBy != null)
                {
                    ((INotifyPropertyChanged) this._groupBy).PropertyChanged += this.OnGroupByChanged;
                }
                if (isBottomLevel != this.IsBottomLevel)
                {
                    this.OnPropertyChanged(new PropertyChangedEventArgs("IsBottomLevel"));
                }
            }
        }

        internal int LastIndex { get; set; }

        internal object SeedItem
        {
            get
            {
                if ((this.ItemCount > 0) && ((this.GroupBy == null) || (this.GroupBy.GroupNames.Count == 0)))
                {
                    var num = 0;
                    var count = this.Items.Count;
                    while (num < count)
                    {
                        var internal2 = this.Items[num] as CollectionViewGroupInternal;
                        if (internal2 == null)
                        {
                            return this.Items[num];
                        }
                        if (internal2.ItemCount > 0)
                        {
                            return internal2.SeedItem;
                        }
                        num++;
                    }
                }
                return DependencyProperty.UnsetValue;
            }
        }

        private CollectionViewGroupInternal Parent { get; set; }

        #endregion

        #region Methods

        internal void Add(object item)
        {
            this.ChangeCounts(item, 1);
            this.ProtectedItems.Add(item);
        }

        internal void Clear()
        {
            this.ProtectedItems.Clear();
            this.FullCount = 1;
            this.ProtectedItemCount = 0;
        }

        internal IEnumerator GetLeafEnumerator()
        {
            return new LeafEnumerator(this);
        }

        internal int Insert(object item, object seed, IComparer comparer)
        {
            var low = (this.GroupBy == null) ? 0 : this.GroupBy.GroupNames.Count;
            var index = this.FindIndex(item, seed, comparer, low, this.ProtectedItems.Count);
            this.ChangeCounts(item, 1);
            this.ProtectedItems.Insert(index, item);
            return index;
        }

        internal object LeafAt(int index)
        {
            var num = 0;
            var count = this.Items.Count;
            while (num < count)
            {
                var internal2 = this.Items[num] as CollectionViewGroupInternal;
                if (internal2 != null)
                {
                    if (index < internal2.ItemCount)
                    {
                        return internal2.LeafAt(index);
                    }
                    index -= internal2.ItemCount;
                }
                else
                {
                    if (index == 0)
                    {
                        return this.Items[num];
                    }
                    index--;
                }
                num++;
            }
            return null;
        }

        internal int LeafIndexFromItem(object item, int index)
        {
            var num = 0;
            var parent = this;
            while (parent != null)
            {
                var num2 = 0;
                var count = parent.Items.Count;
                while (num2 < count)
                {
                    if (((index < 0) && Equals(item, parent.Items[num2])) || (index == num2))
                    {
                        break;
                    }
                    var internal3 = parent.Items[num2] as CollectionViewGroupInternal;
                    num += (internal3 == null) ? 1 : internal3.ItemCount;
                    num2++;
                }
                item = parent;
                parent = parent.Parent;
                index = -1;
            }
            return num;
        }

        internal int LeafIndexOf(object item)
        {
            var num = 0;
            var num2 = 0;
            var count = this.Items.Count;
            while (num2 < count)
            {
                var internal2 = this.Items[num2] as CollectionViewGroupInternal;
                if (internal2 != null)
                {
                    var num4 = internal2.LeafIndexOf(item);
                    if (num4 >= 0)
                    {
                        return (num + num4);
                    }
                    num += internal2.ItemCount;
                }
                else
                {
                    if (Equals(item, this.Items[num2]))
                    {
                        return num;
                    }
                    num++;
                }
                num2++;
            }
            return -1;
        }

        internal int Remove(object item, bool returnLeafIndex)
        {
            var num = -1;
            var index = this.ProtectedItems.IndexOf(item);
            if (index >= 0)
            {
                if (returnLeafIndex)
                {
                    num = this.LeafIndexFromItem(null, index);
                }
                this.ChangeCounts(item, -1);
                this.ProtectedItems.RemoveAt(index);
            }
            return num;
        }

        protected void ChangeCounts(object item, int delta)
        {
            var flag = !(item is CollectionViewGroup);
            for (var internal2 = this; internal2 != null; internal2 = internal2.Parent)
            {
                internal2.FullCount += delta;
                if (flag)
                {
                    internal2.ProtectedItemCount += delta;
                    if (internal2.ProtectedItemCount == 0)
                    {
                        RemoveEmptyGroup(internal2);
                    }
                }
            }
            this._version++;
        }

        protected virtual int FindIndex(object item, object seed, IComparer comparer, int low, int high)
        {
            if (comparer != null)
            {
                var comparer2 = comparer as ListComparer;
                if (comparer2 != null)
                {
                    comparer2.Reset();
                }
                var comparer3 = comparer as CollectionViewGroupComparer;
                if (comparer3 != null)
                {
                    comparer3.Reset();
                }
                var num = low;
                while (num < high)
                {
                    var internal2 = this.ProtectedItems[num] as CollectionViewGroupInternal;
                    var y = (internal2 != null) ? internal2.SeedItem : this.ProtectedItems[num];
                    if ((y != DependencyProperty.UnsetValue) && (comparer.Compare(seed, y) < 0))
                    {
                        return num;
                    }
                    num++;
                }
                return num;
            }
            return high;
        }

        protected virtual void OnGroupByChanged()
        {
            if (this.Parent != null)
            {
                this.Parent.OnGroupByChanged();
            }
        }

        private static void RemoveEmptyGroup(CollectionViewGroupInternal group)
        {
            var parent = group.Parent;
            if (parent != null)
            {
                var groupBy = parent.GroupBy;
                if (parent.ProtectedItems.IndexOf(group) >= groupBy.GroupNames.Count)
                {
                    parent.Remove(group, false);
                }
            }
        }

        private void OnGroupByChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnGroupByChanged();
        }

        #endregion

        // Nested Types
        internal class CollectionViewGroupComparer : IComparer
        {
            #region Fields

            // Fields
            private CollectionViewGroupRoot _group;
            private int _index;

            #endregion

            #region Constructors and Destructors

            // Methods
            internal CollectionViewGroupComparer(CollectionViewGroupRoot group)
            {
                this.ResetGroup(group);
            }

            #endregion

            #region Public Methods and Operators

            public int Compare(object x, object y)
            {
                if (Equals(x, y))
                {
                    return 0;
                }
                var num = (this._group != null) ? this._group.ItemCount : 0;
                while (this._index < num)
                {
                    var collectionViewGroupRoot = this._group;
                    if (collectionViewGroupRoot != null)
                    {
                        var objB = collectionViewGroupRoot.LeafAt(this._index);
                        if (Equals(x, objB))
                        {
                            return -1;
                        }
                        if (Equals(y, objB))
                        {
                            return 1;
                        }
                    }
                    this._index++;
                }
                return 1;
            }

            #endregion

            #region Methods

            internal void Reset()
            {
                this._index = 0;
            }

            internal void ResetGroup(CollectionViewGroupRoot group)
            {
                this._group = group;
                this._index = 0;
            }

            #endregion
        }

        internal class ListComparer : IComparer
        {
            #region Fields

            // Fields
            private int _index;
            private IList _list;

            #endregion

            #region Constructors and Destructors

            // Methods
            internal ListComparer(IList list)
            {
                this.ResetList(list);
            }

            #endregion

            #region Public Methods and Operators

            public int Compare(object x, object y)
            {
                if (Equals(x, y))
                {
                    return 0;
                }
                var num = (this._list != null) ? this._list.Count : 0;
                while (this._index < num)
                {
                    var list = this._list;
                    if (list != null)
                    {
                        var objB = list[this._index];
                        if (Equals(x, objB))
                        {
                            return -1;
                        }
                        if (Equals(y, objB))
                        {
                            return 1;
                        }
                    }
                    this._index++;
                }
                return 1;
            }

            #endregion

            #region Methods

            internal void Reset()
            {
                this._index = 0;
            }

            internal void ResetList(IList list)
            {
                this._list = list;
                this._index = 0;
            }

            #endregion
        }

        private class LeafEnumerator : IEnumerator
        {
            #region Fields

            // Fields
            private object _current;
            private readonly CollectionViewGroupInternal _group;
            private int _index;
            private IEnumerator _subEnum;
            private int _version;

            #endregion

            #region Constructors and Destructors

            // Methods
            public LeafEnumerator(CollectionViewGroupInternal group)
            {
                this._group = group;
                this.DoReset();
            }

            #endregion

            #region Explicit Interface Properties

            // Properties
            object IEnumerator.Current
            {
                get
                {
                    if ((this._index < 0) || (this._index >= this._group.Items.Count))
                    {
                        throw new InvalidOperationException();
                    }
                    return this._current;
                }
            }

            #endregion

            #region Explicit Interface Methods

            bool IEnumerator.MoveNext()
            {
                if (this._group._version != this._version)
                {
                    throw new InvalidOperationException();
                }
                while ((this._subEnum == null) || !this._subEnum.MoveNext())
                {
                    this._index++;
                    if (this._index >= this._group.Items.Count)
                    {
                        return false;
                    }
                    var internal2 = this._group.Items[this._index] as CollectionViewGroupInternal;
                    if (internal2 == null)
                    {
                        this._current = this._group.Items[this._index];
                        this._subEnum = null;
                        return true;
                    }
                    this._subEnum = internal2.GetLeafEnumerator();
                }
                this._current = this._subEnum.Current;
                return true;
            }

            void IEnumerator.Reset()
            {
                this.DoReset();
            }

            #endregion

            #region Methods

            private void DoReset()
            {
                this._version = this._group._version;
                this._index = -1;
                this._subEnum = null;
            }

            #endregion
        }
    }
}