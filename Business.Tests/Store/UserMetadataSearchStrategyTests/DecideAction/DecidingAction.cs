using System;
using Glasswall.IdentityManagementService.Common.Store;
using NUnit.Framework;

namespace Business.Tests.Store.UserMetadataSearchStrategyTests.DecideAction
{
    [TestFixture]
    public class DecidingAction : UserMetadataSearchStrategyTestBase
    {
        private PathAction _output;
        private string _input;

        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();
        }

        [Test]
        [TestCase("/mnt/users/e308d8c2-f079-4c96-9f86-833ecd564a77.json", PathAction.Collect)]
        [TestCase("/mnt/users/no.json", PathAction.Continue)]
        [TestCase("//no.json", PathAction.Continue)]
        [TestCase(null, PathAction.Continue)]
        public void With_Path_Results_In_Correct_Action(string testCase, PathAction testAction)
        {
            _output = ClassInTest.DecideAction(_input = testCase);
            Assert.That(_output, Is.EqualTo(testAction));
        }
    }
}