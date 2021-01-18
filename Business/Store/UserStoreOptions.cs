using System;
using System.IO;
using System.Text;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Store;

namespace Glasswall.IdentityManagementService.Business.Store
{
    public class UserStoreOptions : IFileStoreOptions
    {
        private readonly IIdentityManagementServiceConfiguration _identityManagementServiceConfiguration;

        public UserStoreOptions(IEncryptionHandler encryptionHandler, IIdentityManagementServiceConfiguration identityManagementServiceConfiguration)
        {
            _identityManagementServiceConfiguration = identityManagementServiceConfiguration ?? throw new ArgumentNullException(nameof(identityManagementServiceConfiguration));
            EncryptionHandler = encryptionHandler ?? throw new ArgumentNullException(nameof(encryptionHandler));
        }

        public string RootPath => _identityManagementServiceConfiguration.UserStoreRootPath;
        
        public IEncryptionHandler EncryptionHandler { get; }

        public byte[] EncryptionSecret => Encoding.UTF8.GetBytes(_identityManagementServiceConfiguration.EncryptionSecret);
    }
}