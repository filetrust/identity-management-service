using Glasswall.IdentityManagementService.Common.Store;

namespace Glasswall.IdentityManagementService.Business.Services
{
    public class CollectTopLevelPaths : IPathActions
    {
        public PathAction DecideAction(string path)
        {
            return PathAction.Collect;
        }
    }
}