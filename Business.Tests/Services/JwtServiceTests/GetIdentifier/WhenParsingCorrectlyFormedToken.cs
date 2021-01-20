using NUnit.Framework;

namespace Business.Tests.Services.JwtServiceTests.GetIdentifier
{
    [TestFixture]
    public class WhenParsingCorrectlyFormedToken : JwtServiceTestBase
    {
        private string _output;

        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();

            _output = ClassInTest.GetIdentifier(ValidToken);
        }

        [Test]
        public void Returns_Correct_Identifier()
        {
            Assert.That(_output, Is.EqualTo("e308d8c2-f079-4c96-9f86-833ecd564a77"));
        }
    }
}