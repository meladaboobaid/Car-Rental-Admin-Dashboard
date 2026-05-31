using Car_Rental_Dashboard_Business_Layer;
using Car_Rental_Dashboard_Server.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Car_Rental_Dashboard_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly string _jwtSecret;
        private readonly string _JWT_ISSUER;
        private readonly string _JWT_AUDIENCE;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _jwtSecret = _configuration["JWT_SECRET"];
            _JWT_ISSUER = _configuration["JWT_ISSUER"];
            _JWT_AUDIENCE = _configuration["JWT_AUDIENCE"];

            if (string.IsNullOrWhiteSpace(_jwtSecret))
            {
                // If it's missing, the API will fail immediately when trying to route to this controller
                throw new Exception("JWT secret key is not configured in the environment variables.");
            }
            if (string.IsNullOrWhiteSpace(_JWT_ISSUER))
            {
                // If it's missing, the API will fail immediately when trying to route to this controller
                throw new Exception("JWT issuer is not configured in the environment variables.");
            }
            if (string.IsNullOrWhiteSpace(_JWT_AUDIENCE))
            {
                // If it's missing, the API will fail immediately when trying to route to this controller
                throw new Exception("JWT audience is not configured in the environment variables.");
            }
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }


        /// <summary>
        ///  This endpoint handles user login.
        ///  It verifies credentials and returns a JWT token(Access & Refresh) if login succeeds.
        ///  </summary>
        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var usersList = await users.GetAllUsersAsync();

            var user = usersList.FirstOrDefault(s => s.email == request.email);

            if (user == null)
            {
                _logger.LogWarning("Failed login attempt (email not found). Email={Email}, IP={IP}", request.email, ip);
                return Unauthorized("Invalid credentials");
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.password, user.password);


            if (!isValidPassword)
            {
                _logger.LogCritical("Failed login attempt (Wrong credentials). Email={Email}, IP={IP}", request.email, ip);
                return Unauthorized("Invalid credentials");
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _JWT_ISSUER,
                audience: _JWT_AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            user.refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            user.refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.refreshTokenRevokedAt = null;

            users LoggedInUser = await users.FindAsync(user.id);

            LoggedInUser.refreshTokenHash = user.refreshTokenHash;
            LoggedInUser.refreshTokenExpiresAt = user.refreshTokenExpiresAt;
            LoggedInUser.refreshTokenRevokedAt = user.refreshTokenRevokedAt;

            await LoggedInUser.SaveAsync();

            _logger.LogInformation("Successful login. Student ID = {ID} Email={Email}, IP={IP}", user.id, request.email, ip);

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            });
        }


        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var usersList = await users.GetAllUsersAsync();
            var user = usersList.FirstOrDefault(s => s.email == request.Email);

            if (user == null)
            {
                _logger.LogWarning("Invalid refresh attempt (email not found). Email={Email}, IP={IP}", request.Email, ip);
                return Unauthorized("Invalid refresh request");
            }

            if (user.refreshTokenRevokedAt != null)
            {
                _logger.LogWarning("Refresh attempt using revoked token. UserId={UserId}, Email={Email}, IP={IP}", user.id, request.Email, ip);
                return Unauthorized("Refresh token is revoked");
            }

            if (user.refreshTokenExpiresAt == null || user.refreshTokenExpiresAt <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh attempt using expired token. UserId={UserId}, Email={Email}, IP={IP}", user.id, request.Email, ip);
                return Unauthorized("Refresh token is expired");
            }

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.refreshTokenHash);

            if (!refreshValid)
            {
                _logger.LogWarning("Invalid refresh token attempt. UserId={UserId}, Email={Email}, IP={IP}", user.id, request.Email, ip);
                return Unauthorized("Invalid refresh token");
            }


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email),
                new Claim(ClaimTypes.Role, user.role)
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var jwt = new JwtSecurityToken(
                issuer: _JWT_ISSUER,
                audience: _JWT_AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                //expires: DateTime.Now.AddSeconds(25),
                signingCredentials: creds
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            var newRefreshToken = GenerateRefreshToken();

            user.refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            user.refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);
            user.refreshTokenRevokedAt = null;

            users LoggedInUser = await users.FindAsync(user.id);
            
            LoggedInUser.refreshTokenHash = user.refreshTokenHash;
            LoggedInUser.refreshTokenExpiresAt = user.refreshTokenExpiresAt;
            LoggedInUser.refreshTokenRevokedAt = user.refreshTokenRevokedAt;

            await LoggedInUser.SaveAsync();

            return Ok(new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }

        [HttpPost("logout")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {

            var UsersList = await users.GetAllUsersAsync();

            var user = UsersList.FirstOrDefault(s => s.email == request.Email);

            if (user == null)
                return Ok();

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.refreshTokenHash);

            if (!refreshValid)
                return Ok();

            user.refreshTokenRevokedAt = DateTime.UtcNow;

            users LoggedOutUser = await users.FindAsync(user.id);
            LoggedOutUser.refreshTokenRevokedAt = user.refreshTokenRevokedAt;

            await LoggedOutUser.SaveAsync();
            return Ok("Logged out successfully");
        }
    
    
    }
}
