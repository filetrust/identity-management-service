using System;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.JwtServiceTests.ValidateSignature
{
    [TestFixture]
    public class WhenValidatingSignature : JwtServiceTestBase
    {
        private string _token;

        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();

            _token = ClassInTest.GetToken(Guid.NewGuid().ToString(),
                "MyVeryLongSecretThatIsVeryGoodBecauseItIsVeryLong", TimeSpan.FromDays(1));
        }

        [Test]
        public void Returns_True_For_Valid_Sig()
        {
            var output = ClassInTest.ValidateSignature(_token, "MyVeryLongSecretThatIsVeryGoodBecauseItIsVeryLong");

            Assert.That(output, Is.True);
        }

        [Test]
        public void Returns_False_For_Invalid_Sig()
        {
            var output = ClassInTest.ValidateSignature(_token, "I AM CLEARLY NOT CORRECT");

            Assert.That(output, Is.False);
        }
    }
}