using Kontakty.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kontakty.Services
{
    public class AuthorizationService
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _databaseContext;

        public AuthorizationService(IConfiguration configuration, DatabaseContext databaseContext)
        {
            _configuration = configuration;
            _databaseContext = databaseContext;
        }

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("to jest moj super tajny klucz"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: "Kontakty",
                audience: "Kontakty",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateUserCredentials(string email, string password)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            return user != null;
        }
    }
}
