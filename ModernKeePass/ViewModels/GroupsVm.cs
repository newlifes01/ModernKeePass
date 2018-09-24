using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernKeePass.Annotations;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class GroupsVm : INotifyPropertyChanged
    {
        private string _title;
        private string _newGroupName;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        // TODO: check why binding not working
        public string NewGroupName
        {
            get => _newGroupName;
            set
            {
                _newGroupName = value;
                OnPropertyChanged();
            }
        }

        public GroupItem RootItem { get; set; }

        public GroupsVm()
        { }

        public GroupsVm(PwGroup group)
        {
            Title = group.Name;
            RootItem = new GroupItem(group, null);
        }

        public void AddNewGroup(string groupName = "")
        {
            var pwGroup = new PwGroup(true, true, groupName, PwIcon.Folder);
            RootItem.Children.Add(new GroupItem(pwGroup, RootItem));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}