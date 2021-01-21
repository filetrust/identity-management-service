using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.IdentityModel.Tokens;

namespace Glasswall.IdentityManagementService.Business.Services
{
    public class JwtTokenService : ITokenService
    {
        public string GetToken(string identifier, string tokenSecret, TimeSpan lifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.Name, identifier)
                }),
                Expires = DateTime.UtcNow.Add(lifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public string GetIdentifier(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var readToken = tokenHandler.ReadJwtToken(token);

            var name = readToken.Claims.FirstOrDefault(f => f.Type == "unique_name")?.Value;
            return name;
        }

        public bool ValidateSignature(string token, string tokenSecret)
        {
            var parts = token.Split(".".ToCharArray());
            var (header, payload, signature) = (parts[0], parts[1], parts[2]);
            var alg = new HMACSHA256(Encoding.UTF8.GetBytes(tokenSecret));
            var hash = alg.ComputeHash(Encoding.UTF8.GetBytes(string.Join(".", header, payload)));
            var computedSignature = Base64UrlEncode(hash);

            return signature == computedSignature;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; 
            output = output.Replace('+', '-'); 
            output = output.Replace('/', '_'); 
            return output;
        }
    }
}