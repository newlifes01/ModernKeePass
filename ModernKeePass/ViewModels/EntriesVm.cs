using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernKeePass.Annotations;
using ModernKeePassLib;

namespace ModernKeePass.ViewModels
{
    public class EntriesVm : INotifyPropertyChanged
    {
        private readonly GroupItem _group;
        private PwEntry _reorderedEntry;
        private EntryItem _selectedEntry;

        public ObservableCollection<EntryItem> Entries { get; set; }

        public EntryItem SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                _selectedEntry = value;
                OnPropertyChanged();
            }
        }

        public EntriesVm(GroupItem parentGroup)
        {
            _group = parentGroup;
            Entries = new ObservableCollection<EntryItem>();
            foreach (var entry in parentGroup.Entries)
            {
                Entries.Add(entry);
            }
            Entries.CollectionChanged += EntriesOnCollectionChanged;
        }

        private void EntriesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    var oldIndex = (uint)e.OldStartingIndex;
                    _reorderedEntry = _group.Group.Entries.GetAt(oldIndex);
                    _group.Group.Entries.RemoveAt(oldIndex);
                    break;
                case NotifyCollectionChangedAction.Add:
                    if (_reorderedEntry == null) _group.Group.AddEntry(((EntryItem)e.NewItems[0]).Entry, true);
                    else _group.Group.Entries.Insert((uint)e.NewStartingIndex, _reorderedEntry);
                    break;
            }
        }

        public void AddNewEntry(string text)
        {
            var entry = new EntryItem(new PwEntry(true, true), _group) {Name = text};
            Entries.Add(entry);
            SelectedEntry = entry;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}