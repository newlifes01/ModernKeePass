using Windows.UI.Xaml.Navigation;
using ModernKeePass.ViewModels;

namespace ModernKeePass.Views
{
    public partial class EntryPage
    {
        public EntryItem ViewModel { get; set; }

        public EntryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is EntryItem entry) ViewModel = entry;
        }
    }
}