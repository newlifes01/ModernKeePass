using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernKeePass.Annotations;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class GroupItem: INotifyPropertyChanged
    {
        private PwGroup _reorderedGroup;
        private bool _isEditMode;

        public PwGroup Group { get; }
        public GroupItem Parent { get; }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => Group.Name;
            set
            {
                Group.Name = value;
                OnPropertyChanged();
            }
        }
        
        public IEnumerable<EntryItem> SubEntries
        {
            get
            {
                var subEntries = new List<EntryItem>();
                subEntries.AddRange(Entries);
                foreach (var group in Children)
                {
                    subEntries.AddRange(group.SubEntries);
                }

                return subEntries;
            }
        }

        public int Symbol => (int) Group.IconId;
        public List<EntryItem> Entries { get; }
        public ObservableCollection<GroupItem> Children { get; set; }

        public GroupItem(PwGroup group, GroupItem parent)
        {
            Group = group;
            Parent = parent;

            Entries = new List<EntryItem>();
            foreach (var entry in group.Entries)
            {
                Entries.Add(new EntryItem(entry, this));
            }

            Children = new ObservableCollection<GroupItem>();
            foreach (var subGroup in group.Groups)
            {
                Children.Add(new GroupItem(subGroup, this));
            }
            Children.CollectionChanged += Children_CollectionChanged;
        }

        // TODO: not triggered when reordering
        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    var oldIndex = (uint)e.OldStartingIndex;
                    _reorderedGroup = Group.Groups.GetAt(oldIndex);
                    Group.Groups.RemoveAt(oldIndex);
                    break;
                case NotifyCollectionChangedAction.Add:
                    if (_reorderedGroup == null) Group.AddGroup(((GroupItem)e.NewItems[0]).Group, true);
                    else Group.Groups.Insert((uint)e.NewStartingIndex, _reorderedGroup);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}