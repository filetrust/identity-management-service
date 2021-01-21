using System;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.JwtServiceTests.GetToken
{
    [TestFixture]
    public class WhenRequestingToken : JwtServiceTestBase
    {
        private string _output;

        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();

            _output = ClassInTest.GetToken(Guid.NewGuid().ToString(),
                "MyVeryLongSecretThatIsVeryGoodBecauseItIsVeryLong", TimeSpan.FromDays(1));
        }

        [Test]
        public void Returns_Correct_Identifier()
        {
            Assert.That(_output, Is.Not.Null);
        }
    }
}