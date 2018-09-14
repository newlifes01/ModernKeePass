﻿using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ModernKeePass.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ModernKeePass.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public new MainVm Model => (MainVm)DataContext;
        
        public MainPage()
        {
            InitializeComponent();
            ListView = MenuListView;
            ListViewSource = MenuItemsSource;
        }

        private new void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            base.ListView_SelectionChanged(sender, e);

            if (!(Model.SelectedItem is MainMenuItemVm selectedItem)) MenuFrame.Navigate(typeof(WelcomePage));
            else selectedItem.Destination.Navigate(selectedItem.PageType, selectedItem.Parameter);
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var file = e.Parameter as StorageFile;
            DataContext = new MainVm(Frame, MenuFrame, file);
        }
    }
}
