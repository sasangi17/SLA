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

            if (string.IsNullOrWhiteSpace(loginDto.Email) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new
                {
                    message = "Email and password are required."
                });
            }

            var user = _authBusiness.Login(
                loginDto.Email.Trim(),
                loginDto.Password
            );

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }


            var roles = user.Roles
                .Select(role => role.Trim().ToLowerInvariant())
                .Distinct()
                .Select(role => role switch
                {
                    "admin" => "Admin",
                    "user" => "User",
                    _ => string.Empty
                })
                .ToList();

            if (roles.Count == 0 || roles.Any(role => role == ""))
            {
                return StatusCode(500, new
                {
                    message = "Invalid role configuration for this account."
                });
            }


            string? selectedRole = null;

            if (!string.IsNullOrWhiteSpace(loginDto.SelectedRole))
            {
                selectedRole = roles.FirstOrDefault(role =>
                    role.Equals(
                        loginDto.SelectedRole.Trim(),
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                // Never issue a token for an unassigned role.
                if (selectedRole == null)
                {
                    return StatusCode(403, new
                    {
                        message = "You are not assigned to this role."
                    });
                }
            }
            else if (roles.Count == 1)
            {
                // Automatically select the only assigned role.
                selectedRole = roles[0];
            }
            else
            {
                // Let Angular ask the user which role to use.
                return Ok(new
                {
                    requiresRoleSelection = true,
                    roles
                });
            }

            // Generate JWT
            var token = _jwtService.GenerateToken(user, selectedRole);

            // Response

            return Ok(new
            {
                requiresRoleSelection = false,
                token = token,
                id = user.Id,
                staffId = user.StaffId,
                email = user.Email,
                fullName = user.FullName,
                role = selectedRole,
                roles
            });
        }
    }
}