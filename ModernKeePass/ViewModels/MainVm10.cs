using Windows.Storage;
using ModernKeePass.Interfaces;
using ModernKeePass.Services;

namespace ModernKeePass.ViewModels
{
    public class MainVm10
    {
        private readonly IDatabaseService _database;
        private readonly IRecentService _recent;

        public IStorageFile File { get; set; }

        public bool IsDatabaseOpened => _database != null && _database.IsOpen;

        public bool HasRecentItems => !IsDatabaseOpened && _recent.EntryCount > 0;

        public string OpenedDatabaseName => _database != null ? _database.Name : string.Empty;

        public MainVm10() : this(DatabaseService.Instance, RecentService.Instance)
        { }

        public MainVm10(IDatabaseService database, IRecentService recent)
        {
            _database = database;
            _recent = recent;
        }
    }
}