using Microsoft.AspNetCore.Mvc;
using Niwahana_backend.Business;
using Niwahana_backend.Models.Dto;
using Niwahana_backend.Services;

namespace Niwahana_backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthBusiness _authBusiness;
        private readonly JwtService _jwtService;

        public AuthController(AuthBusiness authBusiness,JwtService jwtService)
        {
            _authBusiness = authBusiness;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest(new
                {
                    message = "Login data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(loginDto.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required."
                });
            }

            if (string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new
                {
                    message = "Password is required."
                });
            }

            // Database login
            var user = _authBusiness.Login(loginDto.Email.Trim(),loginDto.Password);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Generate JWT
            var token = _jwtService.GenerateToken(user);

            // Response

            return Ok(new
            {
                token = token,
                id = user.Id,
                staffId = user.StaffId,
                email = user.Email,
                fullName = user.FullName,

            });
        }
    }
}