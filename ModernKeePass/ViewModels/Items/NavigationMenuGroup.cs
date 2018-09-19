using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class NavigationMenuGroup
    {
        private readonly PwGroup _group;

        public string Text => _group.Name;
        public int Symbol => (int) _group.IconId;
        public IEnumerable<PwEntry> Entries => _group.Entries;
        public ObservableCollection<NavigationMenuGroup> Children { get; set; }

        public NavigationMenuGroup(PwGroup group)
        {
            _group = group;
            Children = new ObservableCollection<NavigationMenuGroup>();
            foreach (var subGroup in group.Groups)
            {
                Children.Add(new NavigationMenuGroup(subGroup));
            }
        }
    }
}