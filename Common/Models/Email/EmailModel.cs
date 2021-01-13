using System.Collections.Generic;

namespace Glasswall.IdentityManagementService.Common.Models.Email
{
    public abstract class EmailModel
    {
        public abstract string Body { get; }

        public abstract string Subject { get; }

        public abstract string EmailFrom { get; }

        public abstract IEnumerable<string> EmailTo { get; }
    }
}
