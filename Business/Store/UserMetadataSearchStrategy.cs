using Glasswall.IdentityManagementService.Common.Store;
using System;
using System.Linq;

namespace Glasswall.IdentityManagementService.Business.Store
{
    public class UserMetadataSearchStrategy : IPathActions
    {
        public PathAction DecideAction(string path)
        {
            if (path?.Split("/")?.LastOrDefault().Split(".").FirstOrDefault(x => Guid.TryParse(x, out _)) == null)
                return PathAction.Continue;

            return PathAction.Collect;
        }
    }
}