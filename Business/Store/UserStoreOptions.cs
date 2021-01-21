using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Store;

namespace Glasswall.IdentityManagementService.Business.Store
{
    [ExcludeFromCodeCoverage]
    public class UserStoreOptions : IFileStoreOptions
    {
        private readonly IIdentityManagementServiceConfiguration _identityManagementServiceConfiguration;

        public UserStoreOptions(IEncryptionHandler encryptionHandler,
            IIdentityManagementServiceConfiguration identityManagementServiceConfiguration)
        {
            _identityManagementServiceConfiguration = identityManagementServiceConfiguration;
            EncryptionHandler = encryptionHandler;
        }

        public string RootPath => _identityManagementServiceConfiguration.UserStoreRootPath;

        public IEncryptionHandler EncryptionHandler { get; }

        public byte[] EncryptionSecret =>
            Encoding.UTF8.GetBytes(_identityManagementServiceConfiguration.EncryptionSecret);
    }
}