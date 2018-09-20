using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class NavigationMenuGroup
    {
        private PwGroup _reorderedGroup;

        public PwGroup Group { get; }

        public string Text
        {
            get => Group.Name;
            set => Group.Name = value;
        }

        public int Symbol => (int) Group.IconId;
        public IEnumerable<PwEntry> Entries => Group.Entries;
        public ObservableCollection<NavigationMenuGroup> Children { get; set; }

        public NavigationMenuGroup(PwGroup group)
        {
            Group = group;
            Children = new ObservableCollection<NavigationMenuGroup>();
            foreach (var subGroup in group.Groups)
            {
                Children.Add(new NavigationMenuGroup(subGroup));
            }
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    var oldIndex = (uint)e.OldStartingIndex;
                    _reorderedGroup = Group.Groups.GetAt(oldIndex);
                    Group.Groups.RemoveAt(oldIndex);
                    break;
                case NotifyCollectionChangedAction.Add:
                    if (_reorderedGroup == null) Group.AddGroup(((NavigationMenuGroup)e.NewItems[0]).Group, true);
                    else Group.Groups.Insert((uint)e.NewStartingIndex, _reorderedGroup);
                    break;
            }
        }
    }
}