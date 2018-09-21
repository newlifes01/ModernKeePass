using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernKeePass.Annotations;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class NavigationMenuGroup: INotifyPropertyChanged
    {
        private PwGroup _reorderedGroup;
        private bool _isEditMode;

        public PwGroup Group { get; }
        public NavigationMenuGroup Parent { get; }

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

        public int Symbol => (int) Group.IconId;
        public IEnumerable<PwEntry> Entries => Group.Entries;
        public ObservableCollection<NavigationMenuGroup> Children { get; set; }

        public NavigationMenuGroup(PwGroup group, NavigationMenuGroup parent)
        {
            Group = group;
            Parent = parent;
            Children = new ObservableCollection<NavigationMenuGroup>();
            foreach (var subGroup in group.Groups)
            {
                Children.Add(new NavigationMenuGroup(subGroup, this));
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
                    if (_reorderedGroup == null) Group.AddGroup(((NavigationMenuGroup)e.NewItems[0]).Group, true);
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