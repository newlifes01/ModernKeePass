using Windows.Storage;
using ModernKeePass.Interfaces;
using ModernKeePass.Services;

namespace ModernKeePass.ViewModels
{
    public class MainVm10
    {
        private readonly IDatabaseService _database;
        private readonly IRecentService _recent;
        private readonly IStorageFile _databaseFile;

        public bool IsDatabaseOpened => _database != null && _database.IsOpen;

        public bool IsRecentSelected => !IsDatabaseOpened && _recent.EntryCount > 0;

        public string OpenedDatabaseName => _database != null ? _database.Name : string.Empty;

        public MainVm10() { }

        internal MainVm10(IStorageFile databaseFile = null) : this(DatabaseService.Instance, RecentService.Instance, databaseFile)
        { }

        public MainVm10(IDatabaseService database, IRecentService recent, IStorageFile databaseFile = null)
        {
            _database = database;
            _recent = recent;
            _databaseFile = databaseFile;
        }
    }
}