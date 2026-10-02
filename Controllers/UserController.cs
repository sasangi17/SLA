using Microsoft.AspNetCore.Mvc;
using Niwahana_backend.Business;
using Niwahana_backend.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Niwahana_backend.Models.Dto;

namespace Niwahana_backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness _userBusiness;
        public UserController(UserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [HttpGet("getuser")]
        public IActionResult GetUser(int userId)
        {
            var user = _userBusiness.GetUserById(userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Position = user.Position,
                Department = user.Department,
                MobilePhone = user.MobilePhone,
                Telephone = user.Telephone,
                Address = user.Address,
                LinkedInLink = user.LinkedInLink,
                StaffId = user.StaffId.ToString()
            };

            return Ok(userDto);
        }

        [HttpPut("updateuser")]
        public IActionResult UpdateUser(int userId,[FromBody] UserMl user)
        {
            if (user == null)
            {
                return BadRequest(new
                {
                    message = "User data is required."
                });
            }

            if (userId != user.Id)
            {
                return BadRequest(new
                {
                    message = "User ID does not match."
                });
            }

            var updated =  _userBusiness.UpdateUser(user);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(new
            {
                message = "User details updated successfully."
            });
        }
    }
}