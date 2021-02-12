using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Glasswall.IdentityManagementService.Api;
using Glasswall.IdentityManagementService.Api.BackgroundServices;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.StartupTests
{
    [TestFixture]
    public class WhenUsingStartup : UnitTestBase<Startup>
    {
        private string _testPath;

        [SetUp]
        public void Setup()
        {
            _testPath = nameof(WhenUsingStartup);

            Directory.CreateDirectory(_testPath);
        }

        [TearDown]
        public void Teardown()
        {
            Directory.Delete(_testPath);
        }

        [Test]
        public void Can_Resolve_Distributer_Service()
        {
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenLifetime),
                TimeSpan.FromDays(1).ToString());
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.EncryptionSecret),
                "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.ManagementUIEndpoint),
                "nameyname");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenSecret), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.UserStoreRootPath),
                _testPath);

            ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

            var services = new ServiceCollection();

            ClassInTest.ConfigureServices(services);

            Assert.That(services.Any(s =>
                s.ServiceType == typeof(IFileStore)), "No file store was added");
        }

        [Test]
        public void Can_Resolve_DefaultUserBackgroundService()
        {
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenLifetime), TimeSpan.FromDays(1).ToString());
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.EncryptionSecret), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.ManagementUIEndpoint), "nameyname");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenSecret), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.UserStoreRootPath), _testPath);

            ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

            var services = new ServiceCollection();

            ClassInTest.ConfigureServices(services);

            Assert.That(services.Any(s => s.ImplementationType == typeof(DefaultUserBackgroundService)), "No background service was added");
        }

        [Test]
        public void Configuration_Can_Be_Parsed()
        {
            var services = new ServiceCollection();

            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenLifetime),
                TimeSpan.FromDays(1).ToString());
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.EncryptionSecret),
                "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.ManagementUIEndpoint),
                "nameyname");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenSecret), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.UserStoreRootPath),
                _testPath);

            ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

            ClassInTest.ConfigureServices(services);

            var config = services.BuildServiceProvider().GetRequiredService<IIdentityManagementServiceConfiguration>();

            Assert.That(config.UserStoreRootPath, Is.EqualTo(_testPath));
        }

        [Test]
        public void When_Mounted_Store_Is_Missing()
        {
            var services = new ServiceCollection();

            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenLifetime),
                TimeSpan.FromDays(1).ToString());
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.EncryptionSecret),
                "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.ManagementUIEndpoint),
                "nameyname");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.TokenSecret), "keymckey");
            Environment.SetEnvironmentVariable(nameof(IIdentityManagementServiceConfiguration.UserStoreRootPath),
                "I DO NOT EXIST");

            ClassInTest = new Startup(new ConfigurationBuilder().AddEnvironmentVariables().Build());

            Assert.That(() => ClassInTest.ConfigureServices(services),
                Throws.Exception.InstanceOf<ConfigurationErrorsException>());
        }

        [Test]
        public void When_ConfigValue_Is_Missing()
        {
            var services = new ServiceCollection();

            ClassInTest = new Startup(new ConfigurationBuilder().Build());

            Assert.That(() => ClassInTest.ConfigureServices(services),
                Throws.Exception.InstanceOf<ConfigurationErrorsException>());
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Params()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<Startup>();
        }

        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<Startup>();
        }
    }
}