using System;
using System.Collections.ObjectModel;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class GroupsVm
    {
        public ObservableCollection<NavigationMenuGroup> MainMenu { get; set; }

        public GroupsVm()
        { }

        public GroupsVm(PwGroup group)
        {
            MainMenu = new ObservableCollection<NavigationMenuGroup>();
            foreach (var subGroup in group.Groups)
            {
                MainMenu.Add(new NavigationMenuGroup(subGroup));
            }
        }
        
    }
}