using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernKeePass.Annotations;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class GroupsVm : INotifyPropertyChanged
    {
        private readonly PwGroup _rootGroup;
        private PwGroup _reorderedGroup;
        private string _title;
        private bool _isEditMode;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<NavigationMenuGroup> MainMenu { get; set; }

        public GroupsVm()
        { }

        public GroupsVm(PwGroup group)
        {
            _rootGroup = group;
            Title = group.Name;
            MainMenu = new ObservableCollection<NavigationMenuGroup>();
            foreach (var subGroup in group.Groups)
            {
                MainMenu.Add(new NavigationMenuGroup(subGroup));
            }
            MainMenu.CollectionChanged += MainMenu_CollectionChanged;
        }

        public void AddNewGroup(string name = "")
        {
            var pwGroup = new PwGroup(true, true, name, PwIcon.Folder);
            MainMenu.Add(new NavigationMenuGroup(pwGroup));
        }

        private void MainMenu_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    var oldIndex = (uint)e.OldStartingIndex;
                    _reorderedGroup = _rootGroup.Groups.GetAt(oldIndex);
                    _rootGroup.Groups.RemoveAt(oldIndex);
                    break;
                case NotifyCollectionChangedAction.Add:
                    if (_reorderedGroup == null) _rootGroup.AddGroup(((NavigationMenuGroup)e.NewItems[0]).Group, true);
                    else _rootGroup.Groups.Insert((uint)e.NewStartingIndex, _reorderedGroup);
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