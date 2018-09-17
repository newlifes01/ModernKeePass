using ModernKeePass.Interfaces;
using ModernKeePass.Services;

namespace ModernKeePass.ViewModels
{
    public class SettingsVm10
    {
        private readonly IDatabaseService _database;

        public bool IsDatabaseOpened => _database != null && _database.IsOpen;

        public SettingsVm10() : this(DatabaseService.Instance)
        { }

        public SettingsVm10(IDatabaseService database)
        {
            _database = database;
        }
    }
}