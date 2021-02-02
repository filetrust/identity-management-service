using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Services.Exceptions;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.CreateAsync
{
    [TestFixture]
    public class WhenCreatingUserWithDuplicateData : UserMetadataSearchStrategyTestBase
    {
        private UserEditOperationState _output;
        private MemoryStream _memoryStream;
        private User _inputUser;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] {$"{ValidUser.Id}.json"}.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.Is<string>(f => f == $"{ValidUser.Id}.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_memoryStream =
                    new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));

            _inputUser = CreateUser();
            _inputUser.Email = ValidUser.Email;
            _inputUser.Username = ValidUser.Username;

            _output = await ClassInTest.CreateAsync(_inputUser, TestCancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _memoryStream?.Dispose();
        }

        [Test]
        public void Output_Is_Returned()
        {
            Assert.That(_output.User, Is.EqualTo(_inputUser));
            Assert.That(_output.Errors.SelectMany(f => f.Value), Has.Exactly(2).InstanceOf<UserDataNotUniqueError>());
            Assert.That(_output.Errors.SelectMany(f => f.Value), Has.One.With.Property(nameof(UserDataNotUniqueError.ErrorMessage)).EqualTo($"Username: '{_inputUser.Username}' is already taken by user with id '{ValidUser.Id}'"));
            Assert.That(_output.Errors.SelectMany(f => f.Value), Has.One.With.Property(nameof(UserDataNotUniqueError.ErrorMessage)).EqualTo($"Email: '{_inputUser.Email}' is already taken by user with id '{ValidUser.Id}'"));
        }
    }
}