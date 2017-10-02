﻿using System;
using ModernKeePass.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.ViewModels;

// The Group Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234229

namespace ModernKeePass.Pages
{
    /// <summary>
    /// A page that displays an overview of a single group, including a preview of the items
    /// within the group.
    /// </summary>
    public sealed partial class GroupDetailPage : Page
    {
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper { get; }


        public GroupDetailPage()
        {
            InitializeComponent();
            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// 
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(e);

            if (!(e.Parameter is GroupVm)) return;
            DataContext = (GroupVm) e.Parameter;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupVm selectedItem = null;
            if (sender is GridView)
            {
                var gridView = (GridView) sender;
                if (gridView.SelectedIndex == 0)
                {
                    var currentGroup = DataContext as GroupVm;
                    currentGroup?.CreateNewGroup();
                    gridView.SelectedIndex = -1;
                    // TODO: Navigate to new group?
                    return;
                }
                selectedItem = gridView.SelectedItem as GroupVm;
            }
            if (sender is ListView) selectedItem = ((ListView) sender).SelectedItem as GroupVm;
            if (selectedItem == null) return;
            Frame.Navigate(typeof(GroupDetailPage), selectedItem);
        }
        

        private void entriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null && listView.SelectedIndex == -1) return;
            if (listView.SelectedIndex == 0)
            {
                var currentGroup = DataContext as GroupVm;
                currentGroup?.CreateNewEntry();
                listView.SelectedIndex = -1;
                // TODO: Navigate to new entry?
                return;
            }
            Frame.Navigate(typeof(EntryDetailPage), listView?.SelectedItem as EntryVm);
        }

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var group = DataContext as GroupVm;
            group?.RemoveGroup();
            if (Frame.CanGoBack) Frame.GoBack();
        }
    }
}
