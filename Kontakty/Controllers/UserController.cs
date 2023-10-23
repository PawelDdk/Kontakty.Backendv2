using Kontakty.Dto.login;
using Kontakty.Dto.Register;
using Kontakty.Models;
using Kontakty.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kontakty.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AuthorizationService _authorizationService;
        private readonly DatabaseContext _databaseContext;

        public UserController(IConfiguration configuration, AuthorizationService authorizationService, DatabaseContext databaseContext)
        {
            _configuration = configuration;
            _authorizationService = authorizationService;
            _databaseContext = databaseContext;
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
                return BadRequest("Wrong email or password.");

            if (!_authorizationService.ValidateUserCredentials(loginRequest.Email, loginRequest.Password))
                return Unauthorized("Wrong email or password.");

            var user = new User { Email = loginRequest.Email };
            var token = _authorizationService.GenerateJwtToken(user);
            return Ok( new { Token = token });
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
            var token = _authorizationService.GenerateJwtToken(user);

            return Ok(new { Token = token });
        }
    }
}
