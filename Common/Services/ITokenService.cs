namespace Glasswall.IdentityManagementService.Common.Services
{
    public interface ITokenService
    {
        string GetToken(string identifier);
    }
}