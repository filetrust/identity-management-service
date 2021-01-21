using System;
using System.IO;
using System.Linq;
using System.Threading;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Store;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests
{
    public class FileStoreTestBase : UnitTestBase<FileStore>
    {
        protected string RootPath;
        protected Mock<ILogger<FileStore>> Logger;
        protected CancellationToken CancellationToken;

        protected void SharedSetup(string rootPath = null, IEncryptionHandler encrypter = null)
        {
            rootPath ??= $".{Path.DirectorySeparatorChar}{Guid.NewGuid()}";
            RootPath = rootPath;
            Logger = new Mock<ILogger<FileStore>>();
            Logger = new Mock<ILogger<FileStore>>();
            CancellationToken = new CancellationToken(false);
            ClassInTest = new FileStore(Logger.Object, new UserStoreOptions(encrypter,
                new IdentityManagementServiceConfiguration
                {
                    EncryptionSecret = string.Join(null, Enumerable.Repeat("a", 32)),
                    UserStoreRootPath = RootPath = rootPath
                }));
        }
    }
}