using Kontakty.Dto.login;
using Kontakty.Dto.Register;
using Kontakty.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Kontakty.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseContext _databaseContext;

        public UserController(IConfiguration configuration, DatabaseContext databaseContext)
        {
            _configuration = configuration;
            _databaseContext = databaseContext;
        }

        [HttpPost("/login")]
        public ActionResult<string> Login([FromBody] LoginRequest loginRequest)
        {
            var user = _databaseContext.Users.FirstOrDefault(user => user.Email == loginRequest.Email && user.Password == loginRequest.Password);

            if (user is null)
                return BadRequest();

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(
                                    new SymmetricSecurityKey(key),
                                    SecurityAlgorithms.HmacSha512Signature);

            var subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginRequest.Email),
                new Claim(JwtRegisteredClaimNames.Email, loginRequest.Email),
            });

            var expires = DateTime.UtcNow.AddMinutes(10);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = subject,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
        }

        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (string.IsNullOrEmpty(registerRequest.Email) || string.IsNullOrEmpty(registerRequest.Password))
                return BadRequest("Wrong email or password");

            var user = new User
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            };

            _databaseContext.Users.Add(user);
            _databaseContext.SaveChanges();

            return Ok();
        }
    }
}
