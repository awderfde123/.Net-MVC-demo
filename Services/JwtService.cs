using demo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace demo.Services
{
    public class JwtService
    {
        private readonly string _signKey;
        private readonly string _issuer;

        public JwtService(IConfiguration configuration)
        {
            _signKey = configuration["JwtSettings:SignKey"]!;
            _issuer = configuration["JwtSettings:Issuer"]!;
        }

        public string GenerateToken(string userId, Role role, TimeSpan expiresIn)
        {
            var claims = new[]
            {

        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(ClaimTypes.Role, role.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: null, // 沒有驗證 Audience，可以維持 null 或是設預設值
                claims: claims,
                expires: DateTime.UtcNow.Add(expiresIn),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero // 避免伺服器時間不一致導致驗證通過
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }


        public bool TryParseJwt(string token, out JwtSecurityToken? jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            return jwtToken != null;
        }
    }
}
