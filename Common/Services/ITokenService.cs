﻿using System;

namespace Glasswall.IdentityManagementService.Common.Services
{
    public interface ITokenService
    {
        string GetToken(string identifier, string secret, TimeSpan lifetime);
        string GetIdentifier(string token);
        bool ValidateSignature(string token, string secret);
    }
}