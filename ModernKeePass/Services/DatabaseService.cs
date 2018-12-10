using System;
using Windows.Storage;
using ModernKeePass.Exceptions;
using ModernKeePass.Interfaces;
using ModernKeePass.ViewModels;
using ModernKeePassLib;
using ModernKeePassLib.Cryptography.KeyDerivation;
using ModernKeePassLib.Interfaces;
using ModernKeePassLib.Keys;
using ModernKeePassLib.Serialization;
using Microsoft.AppCenter;

namespace ModernKeePass.Services
{
    public class DatabaseService: SingletonServiceBase<DatabaseService>, IDatabaseService
    {
        private readonly PwDatabase _pwDatabase = new PwDatabase();
        private readonly ISettingsService _settings;
        private StorageFile _databaseFile;
        private GroupVm _recycleBin;
        private CompositeKey _compositeKey;

        public GroupVm RootGroup { get; set; }

        public PwGroup RootGroup10 => _pwDatabase.RootGroup;

        public GroupVm RecycleBin
        {
            get => _recycleBin;
            set
            {
                _recycleBin = value;
                _pwDatabase.RecycleBinUuid = _recycleBin?.IdUuid;
            }
        }

        public PwGroup RecyleBinGroup => _pwDatabase.RootGroup.FindGroup(_pwDatabase.RecycleBinUuid, false);
        
        public string Name => _databaseFile?.Name;
        
        public bool IsRecycleBinEnabled
        {
            get => _pwDatabase.RecycleBinEnabled;
            set => _pwDatabase.RecycleBinEnabled = value;
        }
        
        public PwUuid DataCipher
        {
            get => _pwDatabase.DataCipherUuid;
            set => _pwDatabase.DataCipherUuid = value;
        }

        public PwCompressionAlgorithm CompressionAlgorithm
        {
            get => _pwDatabase.Compression;
            set => _pwDatabase.Compression = value;
        }

        public KdfParameters KeyDerivation
        {
            get => _pwDatabase.KdfParameters;
            set => _pwDatabase.KdfParameters = value;
        }

        public bool IsOpen => _pwDatabase.IsOpen;
        public bool IsFileOpen => !_pwDatabase.IsOpen && _databaseFile != null;
        public bool IsClosed => _databaseFile == null;
        public bool HasChanged { get; set; }
        
        public DatabaseService() : this(SettingsService.Instance)
        {
        }

        public DatabaseService(ISettingsService settings)
        {
            _settings = settings;
        }


        /// <summary>
        /// Open a KeePass database
        /// </summary>
        /// <param name="databaseFile">The database file</param>
        /// <param name="key">The database composite key</param>
        /// <param name="createNew">True to create a new database before opening it</param>
        /// <returns>An error message, if any</returns>
        public void Open(StorageFile databaseFile, CompositeKey key, bool createNew = false)
        {
            try
            {
                if (databaseFile == null)
                {
                    throw new ArgumentNullException(nameof(databaseFile));
                }

                _compositeKey = key ?? throw new ArgumentNullException(nameof(key));
                var ioConnection = IOConnectionInfo.FromFile(databaseFile);
                if (createNew)
                {
                    _pwDatabase.New(ioConnection, key);
                    
                    var fileFormat = _settings.GetSetting<string>("DefaultFileFormat");
                    switch (fileFormat)
                    {
                        case "4":
                            KeyDerivation = KdfPool.Get("Argon2").GetDefaultParameters();
                            break;
                    }
                }
                else _pwDatabase.Open(ioConnection, key, new NullStatusLogger());

                _databaseFile = databaseFile;
                RootGroup = new GroupVm(_pwDatabase.RootGroup, null, IsRecycleBinEnabled ? _pwDatabase.RecycleBinUuid : null);
            }
            catch (InvalidCompositeKeyException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }
        }
        
        public void ReOpen()
        {
            Open(_databaseFile, _compositeKey);
        }

        /// <summary>
        /// Commit the changes to the currently opened database to file
        /// </summary>
        public void Save()
        {
            if (!IsOpen) return;
            try
            {
                _pwDatabase.Save(new NullStatusLogger());
            }
            catch (Exception e)
            {
                throw new SaveException(e);
            }
        }

        /// <summary>
        /// Save the current database to another file and open it
        /// </summary>
        /// <param name="file">The new database file</param>
        public void Save(StorageFile file)
        {
            var oldFile = _databaseFile;
            _databaseFile = file;
            try
            {
                _pwDatabase.SaveAs(IOConnectionInfo.FromFile(_databaseFile), true, new NullStatusLogger());
            }
            catch
            {
                _databaseFile = oldFile;
                throw;
            }
        }

        /// <summary>
        /// Close the currently opened database
        /// </summary>
        public void Close(bool releaseFile = true)
        {
            _pwDatabase?.Close();
            if (releaseFile) _databaseFile = null;
        }

        public void AddDeletedItem(PwUuid id)
        {
            _pwDatabase.DeletedObjects.Add(new PwDeletedObject(id, DateTime.UtcNow));
        }

        public void CreateRecycleBin(string title)
        {
            RecycleBin = RootGroup.AddNewGroup(title);
            RecycleBin.IsSelected = true;
            RecycleBin.IconId = (int)PwIcon.TrashBin;
        }

        public void UpdateCompositeKey(CompositeKey newCompositeKey)
        {
            if (newCompositeKey == null) return;
            _compositeKey = newCompositeKey;
            _pwDatabase.MasterKey = newCompositeKey;
        }
    }
}
