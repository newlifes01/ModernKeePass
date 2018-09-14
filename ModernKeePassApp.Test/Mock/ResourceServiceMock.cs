using System;
using ModernKeePass.Interfaces;

namespace ModernKeePassApp.Test.Mock
{
    [Obsolete]
    class ResourceServiceMock : IResourceService
    {
        public string GetResourceValue(string key)
        {
            return string.Empty;
        }
    }
}
