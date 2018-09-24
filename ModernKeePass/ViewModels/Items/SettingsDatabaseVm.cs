﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModernKeePass.Common;
using ModernKeePass.Interfaces;
using ModernKeePass.Services;
using ModernKeePassLib;
using ModernKeePassLib.Cryptography.Cipher;
using ModernKeePassLib.Cryptography.KeyDerivation;

namespace ModernKeePass.ViewModels
{
    // TODO: implement Kdf settings
    public class SettingsDatabaseVm: NotifyPropertyChangedBase, IHasSelectableObject
    {
        private readonly IDatabaseService _database;
        private GroupVm _selectedItem;

        public bool HasRecycleBin
        {
            get => _database.IsRecycleBinEnabled;
            set
            {
                _database.IsRecycleBinEnabled = value;
                OnPropertyChanged("HasRecycleBin");
            }
        }

        public bool IsNewRecycleBin
        {
            get => _database.RecycleBin == null;
            set
            {
                if (value) _database.RecycleBin = null;
            }
        }

        public ObservableCollection<GroupVm> Groups { get; set; }

        public IEnumerable<string> Ciphers
        {
            get
            {
                for (var inx = 0; inx < CipherPool.GlobalPool.EngineCount; inx++)
                {
                    yield return CipherPool.GlobalPool[inx].DisplayName;
                }
            }
        }

        public int CipherIndex
        {
            get
            {
                for (var inx = 0; inx < CipherPool.GlobalPool.EngineCount; ++inx)
                {
                    if (CipherPool.GlobalPool[inx].CipherUuid.Equals(_database.DataCipher)) return inx;
                }
                return -1;
            }
            set => _database.DataCipher = CipherPool.GlobalPool[value].CipherUuid;
        }

        public IEnumerable<string> Compressions => Enum.GetNames(typeof(PwCompressionAlgorithm)).Take((int)PwCompressionAlgorithm.Count);

        public string CompressionName
        {
            get => Enum.GetName(typeof(PwCompressionAlgorithm), _database.CompressionAlgorithm);
            set => _database.CompressionAlgorithm = (PwCompressionAlgorithm)Enum.Parse(typeof(PwCompressionAlgorithm), value);
        }
        public IEnumerable<string> KeyDerivations => KdfPool.Engines.Select(e => e.Name);

        public string KeyDerivationName
        {
            get => KdfPool.Get(_database.KeyDerivation.KdfUuid).Name;
            set { _database.KeyDerivation = KdfPool.Engines.FirstOrDefault(e => e.Name == value)?.GetDefaultParameters(); } 
        }

        public ISelectableModel SelectedItem
        {
            get { return Groups.FirstOrDefault(g => g.IsSelected); }
            set
            {
                if (_selectedItem == value || IsNewRecycleBin) return;
                if (_selectedItem != null)
                {
                    _selectedItem.IsSelected = false;
                }

                SetProperty(ref _selectedItem, (GroupVm)value);

                if (_selectedItem != null)
                {
                    _selectedItem.IsSelected = true;
                }
            }
        }

        public SettingsDatabaseVm() : this(DatabaseService.Instance) { }

        public SettingsDatabaseVm(IDatabaseService database)
        {
            _database = database;
            Groups = _database?.RootGroup.Groups;
        }
    }
}
