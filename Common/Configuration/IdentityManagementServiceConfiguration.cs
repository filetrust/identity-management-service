using System;

namespace Glasswall.IdentityManagementService.Common.Configuration
{
    public class IdentityManagementServiceConfiguration : IIdentityManagementServiceConfiguration
    {
        public string TokenSecret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }

    public interface IIdentityManagementServiceConfiguration
    {
        string TokenSecret { get; }
        TimeSpan TokenLifetime { get; }
    }
}
