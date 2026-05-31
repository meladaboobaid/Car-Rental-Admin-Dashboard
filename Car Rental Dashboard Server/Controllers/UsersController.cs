using Car_Rental_Dashboard_Business_Layer;
using Car_Rental_Dashboard_Data_Access_Layer;
using DevOne.Security.Cryptography.BCrypt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using BCrypt.Net;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Authorize]
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }


        [Authorize(Roles = "admin, Super_admin")]
        [HttpGet("All", Name = "GetAllUsers")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<usersDTO>>> GetAllUsers()
        {
            List<usersDTO> UsersList = await users.GetAllUsersAsync();

            if (UsersList.Count == 0)
            {
                return NotFound("Users Not Found");
            }

            return Ok(UsersList);
        }


        [HttpGet("{id}", Name = "GetUserByID")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<usersDTO>> GetUserByID(int id, [FromServices] IAuthorizationService authorizationService)
        {
            if (id < 1)
            {
                return BadRequest($"Not Accepted ID {id}");
            }

            users user = await users.FindAsync(id);

            if (user == null)
            {
                return NotFound($"User with ID: {id} not found!");
            }

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                id,
                "UserOwnerOrSuperAdmin");

            if (!authResult.Succeeded)
                return Forbid(); // 403

            usersDTO UserDTO = user.userDTO;

            return Ok(UserDTO);
        }


        [Authorize(Roles = "admin,Super_admin")]
        [HttpPost(Name = "AddNewUser")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<usersDTO>> AddNewUser(usersDTO newUser)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (newUser == null 
            || string.IsNullOrWhiteSpace(newUser.name) 
            || string.IsNullOrWhiteSpace(newUser.email) 
            || string.IsNullOrWhiteSpace(newUser.password)
            || _ValidateIfUserIsAdminOrNot(newUser))
            {
                return BadRequest("Invalid user data!");
            }

            newUser.password = BCrypt.Net.BCrypt.HashPassword(newUser.password);

            users user = new users(newUser);
            user.passoword = newUser.password;

            if (await user.SaveAsync())
            {
                newUser.id = user.id;
                _logger.LogInformation("Admin action succeed. AdminId = {AdminId}, Action = Add-User, TargetId = {TargetId}, IP = {IP}", adminId, newUser.id, ip);
                return CreatedAtRoute("GetUserByID", new { id = newUser.id }, newUser);
            }
            else
            {
                return StatusCode(500, new { Message = "Error Adding User" });
            }
        }


        [Authorize(Roles = "admin,Super_admin")]
        [HttpPut("{id}", Name = "UpdateUser")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<usersDTO>> UpdateUser(int id, usersDTO userDTO)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (userDTO == null || string.IsNullOrWhiteSpace(userDTO.name)
               || string.IsNullOrWhiteSpace(userDTO.email)
               || string.IsNullOrWhiteSpace(userDTO.password)
               || string.IsNullOrWhiteSpace(userDTO.role) )
            {
                return BadRequest("Invalid user data");
            }

            users user = await users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Admin action blocked (invalid id). AdminId = {AdminId}, Action = Update-User, TargetId = {TargetId}, IP = {IP}", adminId, id, ip);
                return NotFound($"User With ID {id} Not Found");
            }

            user.name = userDTO.name;
            user.email = userDTO.email;
            user.role = userDTO.role;
            user.passoword = BCrypt.Net.BCrypt.HashPassword(userDTO.password);


            if (await user.SaveAsync())
            {
                return Ok("User Updated Successfully!");
            }
            else
            {
                return StatusCode(500, new { Message = "Error Updating The User" });
            }
        }


        [Authorize(Roles = "admin,Super_admin")]
        [HttpDelete(Name = "DeleteUser")]
        [EnableRateLimiting("UsersLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

            if (id < 1)
            {
                _logger.LogWarning("Admin action blocked (invalid id). AdminId = {AdminId}, Action = Delete-User, TargetId = {TargetId}, IP = {IP}", adminId, id, ip);
                return BadRequest($"Not Accepted ID: {id}");
            }

            users user = await users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Admin action failed (Target Not Found). AdminId = {AdminId}, Action = Delete-User, TargetId = {TargetId}, IP = {IP}", adminId, id, ip);
                return NotFound($"User Wit ID: {id} Not Found");
            }


            _logger.LogInformation(
            "Admin action started. AdminId={AdminId}, Action=Delete-User, TargetId={TargetId}, TargetEmail={TargetEmail}, IP={IP}",
            adminId, id, user.email, ip);


            if (await users.DeleteUserAsync(id))
            {
                _logger.LogInformation(
                    "Admin action succeed. AdminId = {AdminId}, Action = Delete-User, TargetId = {TargetId}, User Email = {Email}, IP = {IP}", adminId, id, user.email, ip);
                return Ok($"User With ID: {id} Deleted Successfully");
            }
            else
            {
                _logger.LogWarning(
                "Admin action failed. AdminId = {AdminId}, Action = Delete-User, TargetId = {TargetId}, IP = {IP}", adminId, id, ip);
                return NotFound($"User With ID: {id} Not Found");
            }
        }

        private bool _ValidateIfUserIsAdminOrNot(usersDTO user)
        {
            return !(user.role != "Super_admin" || user.role != "admin");
        }
    
    }
}
