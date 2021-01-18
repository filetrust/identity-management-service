using System;

namespace Glasswall.IdentityManagementService.Common.Configuration
{
    public class IdentityManagementServiceConfiguration : IIdentityManagementServiceConfiguration
    {
        public string TokenSecret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
        public string ManagementUIEndpoint { get; set; }
        public string EncryptionSecret { get; set; }
        public string UserStoreRootPath { get; set; }
    }

    public interface IIdentityManagementServiceConfiguration
    {
        string TokenSecret { get; }
        TimeSpan TokenLifetime { get; }
        string ManagementUIEndpoint { get; }
        string EncryptionSecret { get; }
        string UserStoreRootPath { get; }
    }
}
