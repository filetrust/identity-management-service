using Glasswall.IdentityManagementService.Business.Services;
using TestCommon;

namespace Business.Tests.Services.JwtServiceTests
{
    public abstract class JwtServiceTestBase : UnitTestBase<JwtTokenService>
    {
        protected string ValidToken;

        protected void CommonSetup()
        {
            ClassInTest = new JwtTokenService();

            ValidToken =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImUzMDhkOGMyLWYwNzktNGM5Ni05Zjg2LTgzM2VjZDU2NGE3NyIsIm5iZiI6MTYxMDk4MDY2MiwiZXhwIjoxNjEwOTgwNzIyLCJpYXQiOjE2MTA5ODA2NjJ9.PRrAC1_6EcU0LIhJaCUWdkoOqspjgHoOZuaSXmxk-Wc";
        }
    }
}
