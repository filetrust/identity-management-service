using System;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
  public class UserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}