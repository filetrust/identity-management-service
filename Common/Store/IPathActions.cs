namespace Glasswall.IdentityManagementService.Common.Store
{
    public interface IPathActions
    {
        PathAction DecideAction(string path);
    }
}