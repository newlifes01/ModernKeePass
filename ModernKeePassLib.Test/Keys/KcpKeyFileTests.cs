using System;
using System.IO;
using ModernKeePassLib.Keys;
using ModernKeePassLib.Utility;
using Windows.Storage;
using Xunit;

namespace ModernKeePassLib.Test.Keys
{
    public class KcpKeyFileTests
    {
        private const string TestCreateFile = "TestCreate.xml";
        private const string TestKey = "0123456789";

        private const string ExpectedFileStart =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
            "<KeyFile>\r\n" +
            "\t<Meta>\r\n" +
            "\t\t<Version>1.00</Version>\r\n" +
            "\t</Meta>\r\n" +
            "\t<Key>\r\n" +
            "\t\t<Data>";

        private const string ExpectedFileEnd = "\t</Key>\r\n" +
                                       "</KeyFile>";

        [Fact]
        public void TestConstruct()
        {
            var expectedKeyData = new byte[]
            {
                0xC1, 0xB1, 0x12, 0x77, 0x23, 0xB8, 0x99, 0xB8,
                0xB9, 0x3B, 0x1B, 0xFF, 0x6C, 0xBE, 0xA1, 0x5B,
                0x8B, 0x99, 0xAC, 0xBD, 0x99, 0x51, 0x85, 0x95,
                0x31, 0xAA, 0x14, 0x3D, 0x95, 0xBF, 0x63, 0xFF
            };

            var folder = StorageFolder.GetFolderFromPathAsync(Path.GetTempPath()).GetAwaiter().GetResult();
            var file =  folder.CreateFileAsync(TestCreateFile).GetAwaiter().GetResult();
            using (var fs = file.OpenStreamForWriteAsync().GetAwaiter().GetResult())
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(ExpectedFileStart);
                    sw.Write(TestKey);
                    sw.Write(ExpectedFileEnd);
                }
            }

            try
            {
                var keyFile = new KcpKeyFile(file);
                var keyData = keyFile.KeyData.ReadData();
                Assert.True(MemUtil.ArraysEqual(keyData, expectedKeyData));
            }
            finally
            {
                file.DeleteAsync().GetAwaiter().GetResult();
            }
        }

        [Fact]
        public void TestCreate()
        {
            var folder = StorageFolder.GetFolderFromPathAsync(Path.GetTempPath()).GetAwaiter().GetResult();
            var file = folder.CreateFileAsync(TestCreateFile).GetAwaiter().GetResult();
            KcpKeyFile.Create(file, null);
            try
            {
                var fileContents = FileIO.ReadTextAsync(file).GetAwaiter().GetResult();

                Assert.Equal(185, fileContents.Length);
                Assert.StartsWith(ExpectedFileStart, fileContents);
                Assert.EndsWith(ExpectedFileEnd, fileContents);
            }
            finally
            {
                file.DeleteAsync().GetAwaiter().GetResult();
            }
        }
    }
}

