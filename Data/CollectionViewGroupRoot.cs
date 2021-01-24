using System;

namespace Paya.Automation.Editor.Data
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    internal class CollectionViewGroupRoot : CollectionViewGroupInternal, INotifyCollectionChanged
    {
        #region Constants

        private const string RootName = "Root";

        #endregion

        #region Static Fields

        private static readonly object UseAsItemDirectly = new object();
        private static GroupDescription topLevelGroupDescription;

        #endregion

        #region Fields

        // Fields
        private readonly ObservableCollection<GroupDescription> _groupBy;
        private readonly ICollectionView _view;

        #endregion

        #region Constructors and Destructors

        // Methods
        internal CollectionViewGroupRoot(ICollectionView view, bool isDataInGroupOrder) : base(RootName, null)
        {
            this._groupBy = new ObservableCollection<GroupDescription>();
            this._view = view;
            this.IsDataInGroupOrder = isDataInGroupOrder;
        }

        #endregion

        #region Public Events

        // Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Events

        internal event EventHandler GroupDescriptionChanged;

        #endregion

        #region Public Properties

        public virtual GroupDescriptionSelectorCallback GroupBySelector { get; set; }

        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return this._groupBy; }
        }

        #endregion

        #region Properties

        // Properties
        internal IComparer ActiveComparer { get; set; }

        internal CultureInfo Culture
        {
            get { return this._view.Culture; }
        }

        internal bool IsDataInGroupOrder { get; set; }

        #endregion

        #region Public Methods and Operators

        public void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        #endregion

        #region Methods

        internal void AddToSubgroups(object item, bool loading)
        {
            this.AddToSubgroups(item, this, 0, loading);
        }

        internal void Initialize()
        {
            if (topLevelGroupDescription == null)
            {
                topLevelGroupDescription = new TopLevelGroupDescription();
            }
            this.InitializeGroup(this, 0);
        }

        internal void InsertSpecialItem(int index, object item, bool loading)
        {
            this.ChangeCounts(item, 1);
            this.ProtectedItems.Insert(index, item);
            if (!loading)
            {
                var num = this.LeafIndexFromItem(item, index);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, num));
            }
        }

        internal bool RemoveFromSubgroups(object item)
        {
            return this.RemoveFromSubgroups(item, this, 0);
        }

        internal void RemoveItemFromSubgroupsByExhaustiveSearch(object item)
        {
            this.RemoveItemFromSubgroupsByExhaustiveSearch(this, item);
        }

        internal void RemoveSpecialItem(int index, object item, bool loading)
        {
            var num = -1;
            if (!loading)
            {
                num = this.LeafIndexFromItem(item, index);
            }
            this.ChangeCounts(item, -1);
            this.ProtectedItems.RemoveAt(index);
            if (!loading)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, num));
            }
        }

        protected override int FindIndex(object item, object seed, IComparer comparer, int low, int high)
        {
            var view = this._view as IEditableCollectionView;
            if ((view != null) && view.IsAddingNew)
            {
                high--;
            }
            return base.FindIndex(item, seed, comparer, low, high);
        }

        protected override void OnGroupByChanged()
        {
            if (this.GroupDescriptionChanged != null)
            {
                this.GroupDescriptionChanged(this, EventArgs.Empty);
            }
        }

        private void AddToSubgroup(object item, CollectionViewGroupInternal group, int level, object name, bool loading)
        {
            CollectionViewGroupInternal internal2;
            var num = this.IsDataInGroupOrder ? group.LastIndex : 0;
            var count = group.Items.Count;
            while (num < count)
            {
                internal2 = group.Items[num] as CollectionViewGroupInternal;
                if ((internal2 != null) && group.GroupBy.NamesMatch(internal2.Name, name))
                {
                    group.LastIndex = num;
                    this.AddToSubgroups(item, internal2, level + 1, loading);
                    return;
                }
                num++;
            }
            internal2 = new CollectionViewGroupInternal(name, group);
            this.InitializeGroup(internal2, level + 1);
            if (loading)
            {
                group.Add(internal2);
                group.LastIndex = num;
            }
            else
            {
                group.Insert(internal2, item, this.ActiveComparer);
            }
            this.AddToSubgroups(item, internal2, level + 1, loading);
        }

        private void AddToSubgroups(object item, CollectionViewGroupInternal group, int level, bool loading)
        {
            var name = this.GetGroupName(item, group.GroupBy, level);
            if (name == UseAsItemDirectly)
            {
                if (loading)
                {
                    group.Add(item);
                }
                else
                {
                    var index = group.Insert(item, item, this.ActiveComparer);
                    var num2 = group.LeafIndexFromItem(item, index);
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, num2));
                }
            }
            else
            {
                var is2 = name as ICollection;
                if (is2 == null)
                {
                    this.AddToSubgroup(item, group, level, name, loading);
                }
                else
                {
                    foreach (var obj3 in is2)
                    {
                        this.AddToSubgroup(item, group, level, obj3, loading);
                    }
                }
            }
        }

        private GroupDescription GetGroupDescription(CollectionViewGroup group, int level)
        {
            GroupDescription description = null;
            if (group == this)
            {
                group = null;
            }
            if ((this.GroupBySelector != null))
            {
                description = this.GroupBySelector(group, level);
            }
            if ((description == null) && (level < this.GroupDescriptions.Count))
            {
                description = this.GroupDescriptions[level];
            }
            return description;
        }

        private object GetGroupName(object item, GroupDescription groupDescription, int level)
        {
            return groupDescription != null ? groupDescription.GroupNameFromItem(item, level, this.Culture) : UseAsItemDirectly;
        }

        private void InitializeGroup(CollectionViewGroupInternal group, int level)
        {
            var groupDescription = this.GetGroupDescription(group, level);
            group.GroupBy = groupDescription;
            var observables = (groupDescription != null) ? groupDescription.GroupNames : null;
            if (observables != null)
            {
                var num = 0;
                var count = observables.Count;
                while (num < count)
                {
                    var internal2 = new CollectionViewGroupInternal(observables[num], group);
                    this.InitializeGroup(internal2, level + 1);
                    group.Add(internal2);
                    num++;
                }
            }
            group.LastIndex = 0;
        }

        private bool RemoveFromGroupDirectly(CollectionViewGroupInternal group, object item)
        {
            var index = group.Remove(item, true);
            if (index >= 0)
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return false;
            }
            return true;
        }

        private bool RemoveFromSubgroup(object item, CollectionViewGroupInternal group, int level, object name)
        {
            var flag = false;
            var num = 0;
            var count = group.Items.Count;
            while (num < count)
            {
                var internal2 = group.Items[num] as CollectionViewGroupInternal;
                if ((internal2 != null) && group.GroupBy.NamesMatch(internal2.Name, name))
                {
                    if (this.RemoveFromSubgroups(item, internal2, level + 1))
                    {
                        flag = true;
                    }
                    return flag;
                }
                num++;
            }
            return true;
        }

        private bool RemoveFromSubgroups(object item, CollectionViewGroupInternal group, int level)
        {
            var flag = false;
            var name = this.GetGroupName(item, group.GroupBy, level);
            if (name == UseAsItemDirectly)
            {
                return this.RemoveFromGroupDirectly(group, item);
            }
            var is2 = name as ICollection;
            if (is2 == null)
            {
                if (this.RemoveFromSubgroup(item, group, level, name))
                {
                    flag = true;
                }
                return flag;
            }
            if (is2.Cast<object>().Any(obj3 => this.RemoveFromSubgroup(item, @group, level, obj3)))
            {
                flag = true;
            }
            return flag;
        }

        private void RemoveItemFromSubgroupsByExhaustiveSearch(CollectionViewGroupInternal group, object item)
        {
            if (this.RemoveFromGroupDirectly(group, item))
            {
                for (var i = group.Items.Count - 1; i >= 0; i--)
                {
                    var internal2 = group.Items[i] as CollectionViewGroupInternal;
                    if (internal2 != null)
                    {
                        this.RemoveItemFromSubgroupsByExhaustiveSearch(internal2, item);
                    }
                }
            }
        }

        #endregion

        // Nested Types
        private class TopLevelGroupDescription : GroupDescription
        {
            #region Public Methods and Operators

            // Methods
            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                return null;
            }

            #endregion
        }
    }
}