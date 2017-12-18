﻿using System;
using ModernKeePass.Interfaces;
using ModernKeePass.ViewModels;
using ModernKeePassLib;
using ModernKeePassLib.Cryptography.KeyDerivation;
using ModernKeePassLib.Keys;
using Windows.Storage;

namespace ModernKeePassApp.Test.Mock
{
    public class DatabaseServiceMock : IDatabase
    {
        public PwCompressionAlgorithm CompressionAlgorithm { get; set; }

        public StorageFile DatabaseFile { get; set; }

        public PwUuid DataCipher { get; set; }

        public KdfParameters KeyDerivation { get; set; }

        public string Name => "MockDatabase";

        public GroupVm RecycleBin { get; set; }

        public bool RecycleBinEnabled { get; set; }

        public GroupVm RootGroup { get; set; }

        public int Status { get; set; }

        public void AddDeletedItem(PwUuid id)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            Status = 0;
        }

        public void CreateRecycleBin()
        {
            throw new NotImplementedException();
        }

        public void Open(CompositeKey key, bool createNew)
        {
            Status = 2;
        }

        public void Save()
        {
            // Do Nothing
        }

        public void Save(StorageFile file)
        {
            throw new NotImplementedException();
        }

        public void UpdateCompositeKey(CompositeKey key)
        {
            throw new NotImplementedException();
        }
    }
}
