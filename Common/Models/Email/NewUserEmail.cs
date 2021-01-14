using System;
using System.Collections.Generic;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Models.Store;

namespace Glasswall.IdentityManagementService.Common.Models.Email
{
    public class NewUserEmail : EmailModel
    {
        private readonly User _createdUser;
        private readonly IIdentityManagementServiceConfiguration _config;
        private readonly string _confirmEmailToken;

        public NewUserEmail(User createdUser, IIdentityManagementServiceConfiguration config, string confirmEmailToken)
        {
            _createdUser = createdUser ?? throw new System.ArgumentNullException(nameof(createdUser));
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
            _confirmEmailToken = confirmEmailToken ?? throw new ArgumentNullException(nameof(confirmEmailToken));
        }

        public override string Body => GetHtmlBody();

        public override string Subject => "New user notification";

        public override string EmailFrom => "admin@glasswallsolutions.com";

        public override IEnumerable<string> EmailTo => new[] { _createdUser.Email };

        private string GetHtmlBody()
        {
            return $"Please confirm your email <a href=\"{_config.ManagementUIEndpoint}/confirm?token={_confirmEmailToken}\">here</a>";
        }
    }
}