using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Niwahana_backend.Models.Domain;

namespace Niwahana_backend.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserMl user)
        {
            var jwtKey =_configuration["Jwt:Key"]?? throw new Exception("JWT Key is missing.");
            var issuer =_configuration["Jwt:Issuer"]?? throw new Exception("JWT Issuer is missing.");
            var audience =_configuration["Jwt:Audience"]?? throw new Exception("JWT Audience is missing.");
            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]?? "60");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name,user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires:
                DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
