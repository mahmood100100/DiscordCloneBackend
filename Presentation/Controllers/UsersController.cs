using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DiscordCloneBackend.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRequestDTO userDto)
        {
            try
            {
                var (errors, user) = await userService.RegisterUserAsync(userDto);

                if (errors.Any())
                {
                    return BadRequest(new ApiValidationResponse(errors));
                }

                var token = await userService.GenerateEmailConfirmationTokenAsync(user.Email);

                var verificationUrl = Url.Action(
                    "VerifyEmail",
                    "Users",
                    new { email = user.Email, token = token },
                    Request.Scheme,
                    Request.Host.ToString()
                );

                await userService.SendVerificationEmailAsync(userDto.Email, verificationUrl);

                return Ok(new ApiResponse(200, "User registered successfully. Please check your email for confirmation."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
        {
            try
            {
                var loginResponse = await userService.LoginUserAsync(loginDto);

                Response.Cookies.Append("refresh_token", loginResponse.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new ApiResponse(200, "Login successful", new { user = loginResponse.User, accessToken = loginResponse.Token , expiresIn = 3600 }));
            }
            catch (Exception ex)
            {
                return Unauthorized(new ApiResponse(401, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await userService.GetUserByIdAsync(id);
                return Ok(new ApiResponse(200, "User found", user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await userService.GetAllUsersAsync();
                return Ok(new ApiResponse(200, "Users fetched successfully", users));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromForm] UserUpdateDto userDto)
        {
            try
            {
                var userResponse = await userService.UpdateUserAsync(userDto);
                return Ok(new ApiResponse(200, "User updated successfully." , userResponse));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse(409, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, "An unexpected error occurred."));
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await userService.DeleteUserAsync(id);
                return Ok(new ApiResponse(200, "User deleted successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] SendForEmailDTO sendForEmailDto)
        {
            try
            {
                var response = await userService.SendPasswordResetEmailAsync(sendForEmailDto);

                if (response.StatusCode == 200)
                {
                    return Ok(response);
                }

                return BadRequest(response); 
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            try
            {
                if (string.IsNullOrEmpty(resetPasswordDto.Token) || string.IsNullOrEmpty(resetPasswordDto.Email) || string.IsNullOrEmpty(resetPasswordDto.NewPassword))
                {
                    return BadRequest(new ApiValidationResponse(new[] { "Invalid password reset request. Please provide all necessary information." }));
                }

                await userService.ResetPasswordAsync(resetPasswordDto);

                return Ok(new ApiResponse(200, "Password reset successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {

                return Unauthorized(new ApiResponse(401, "Refresh token is missing"));
            }

            try
            {
                var newTokens = await userService.RefreshTokenAsync(refreshToken);
                if (newTokens == null)
                {
                    return Unauthorized(new ApiResponse(401, "Invalid or expired refresh token"));
                }

                Response.Cookies.Append("refresh_token", newTokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                return Ok(new
                {
                    user = newTokens.User,
                    accessToken = newTokens.Token,
                    expiresIn = 3600
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Append("refresh_token", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(-1)
                });

                Response.Cookies.Delete("refresh_token");

                return Ok(new ApiResponse(200, "Logged out successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(500, ex.Message));
            }
        }

        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] SendForEmailDTO emailDto)
        {
            try
            {
                var token = await userService.GenerateEmailConfirmationTokenAsync(emailDto.Email);

                var verificationUrl = Url.Action(
                    "VerifyEmail",
                    "Users",
                    new { email = emailDto.Email, token = token },
                    Request.Scheme,
                    Request.Host.ToString()
                );

                var response = await userService.SendVerificationEmailAsync(emailDto.Email, verificationUrl);

                if (response.StatusCode == 429)
                {
                    return StatusCode(429, response);
                }

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An unexpected error occurred."));
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            var response = await userService.VerifyEmailAsync(email, token);

            if (response.StatusCode == 200)
            {
                return Ok("Email verified successfully!");
            }
            return BadRequest($"{response.Message}");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            try
            {
                await userService.ChangePasswordAsync(request);
                return Ok(new ApiResponse(200, "Password changed successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An unexpected error occurred", ex.Message));
            }
        }
    }

}
