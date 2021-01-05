using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.IdentityModel.Tokens;

namespace Glasswall.IdentityManagementService.Business.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IIdentityManagementServiceConfiguration _configuration;

        public JwtTokenService(IIdentityManagementServiceConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetToken(string identifier)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.TokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identifier)
                }),
                Expires = DateTime.UtcNow.Add(_configuration.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}