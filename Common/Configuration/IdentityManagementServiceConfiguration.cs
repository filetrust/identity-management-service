using System;

namespace Glasswall.IdentityManagementService.Common.Configuration
{
    public class IdentityManagementServiceConfiguration : IIdentityManagementServiceConfiguration
    {
        public string TokenSecret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
        public string ManagementUIEndpoint { get; }
    }

    public interface IIdentityManagementServiceConfiguration
    {
        string TokenSecret { get; }
        TimeSpan TokenLifetime { get; }
        string ManagementUIEndpoint { get; }
    }
}
