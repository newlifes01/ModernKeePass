using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class NavigationMenuGroup
    {
        private readonly PwGroup _group;

        public string Text => _group.Name;
        public int Symbol => (int) _group.IconId;
        public IEnumerable<PwEntry> Entries => _group.Entries;
        public IList<NavigationMenuGroup> Children { get; set; } = new List<NavigationMenuGroup>();

        public NavigationMenuGroup(PwGroup group)
        {
            _group = group;
            foreach (var subGroup in group.Groups)
            {
                Children.Add(new NavigationMenuGroup(subGroup));
            }
        }

        public TreeViewNode AsTreeViewNode()
        {
            var result = new TreeViewNode
            {
                Content = this
            };

            foreach (var subItem in Children)
            {
                result.Children.Add(subItem.AsTreeViewNode());
            }

            return result;
        }
    }
}