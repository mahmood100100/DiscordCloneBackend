using AutoMapper;
using DiscordCloneBackend.Application.Common.Response;
using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Application.IServices;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IExternalServices;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using DiscordCloneBackend.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiscordCloneBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IProfileService _profileService;
        private readonly IServerService _serverService;
        private readonly IConfiguration configuration;

        public UserService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IMapper mapper,
            IEmailService emailService,
            IFileService _fileService,
            IProfileService profileService,
            IServerService _serverService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
            _emailService = emailService;
            this._fileService = _fileService;
            _profileService = profileService;
            this._serverService = _serverService;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllUsersAsync();
            var usersDto = _mapper.Map<IEnumerable<UserResponseDTO>>(users);

            foreach (var userDto in usersDto)
            {
                var user = await _unitOfWork.Users.GetUserByIdAsync(userDto.ID);
                userDto.Roles = (await _unitOfWork.Users.GetRolesAsync(user)).ToList();
            }

            return usersDto;
        }

        public async Task ChangePasswordAsync(ChangePasswordDTO request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            var user = await _unitOfWork.Users.GetUserByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");
            }

            var result = await _unitOfWork.Users.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to change password: {errorMessages}");
            }
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(string id)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(id);
            var userDto = _mapper.Map<UserResponseDTO>(user);
            if (userDto == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            userDto.Roles = (await _unitOfWork.Users.GetRolesAsync(user)).ToList();
            return userDto;
        }

        public async Task<(List<string> Errors, LocalUser User)> RegisterUserAsync(UserRequestDTO request)
        {
            var (isUnique, takenFields) = await CheckUserValidityAsync(request.UserName, request.Email);

            if (!isUnique)
            {
                var errorMessages = takenFields.Select(field => $"{field} is already taken").ToList();
                return (errorMessages, null);
            }

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var user = _mapper.Map<LocalUser>(request);

                await _unitOfWork.Users.CreateUserAsync(user, request.Password);

                var role = "user";
                await _unitOfWork.Users.AddToRoleAsync(user, role);

                await _profileService.CreateProfileAsync(request, user.Id);

                await _unitOfWork.CompleteAsync();

                return (new List<string>(), user);
            });
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO request)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var result = await _unitOfWork.Users.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to reset password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException($"User with email {email} not found.");
            }

            return await _unitOfWork.Users.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ApiResponse> SendPasswordResetEmailAsync(SendForEmailDTO sendForEmailDTO)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(sendForEmailDTO.Email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            var timeRemaining = CanRequestPasswordResetAsync(sendForEmailDTO.Email, user.LastPasswordResetRequested);

            if (timeRemaining.HasValue)
            {
                var remainingMinutes = (int)Math.Ceiling(timeRemaining.Value.TotalMinutes);
                return new ApiResponse(400, $"Please try again in {remainingMinutes} minutes.");
            }

            string resetPasswordToken = await GeneratePasswordResetTokenAsync(sendForEmailDTO.Email);

            if (string.IsNullOrEmpty(resetPasswordToken))
            {
                return new ApiResponse(400, "Failed to generate password reset token.");
            }

            var subject = "Reset Password Request";
            var link = $"{configuration["CorsSettings:FrontendUrl"]}/auth/reset-password";
            var placeholders = new Dictionary<string, string>
    {
       { "{RESET_TOKEN}", resetPasswordToken },
       { "{RESET_LINK}", link }
    };

            try
            {
                await _emailService.SendMailAsync(sendForEmailDTO.Email, subject, "ResetPasswordTemplate.html", placeholders);

                user.LastPasswordResetRequested = DateTime.UtcNow;
                await _unitOfWork.CompleteAsync();

                return new ApiResponse(200, "Password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Failed to send email: {ex.Message}");
            }
        }


        public async Task DeleteUserAsync(string userId)
        {
            var userProfile = await _profileService.GetProfileByUserIdAsync(userId);
            await _serverService.DeleteServersByProfileIdAsync(userProfile.Id);
            await _profileService.DeleteProfileByUserIdAsync(userId);
            await _unitOfWork.Users.DeleteUserAsync(userId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<(bool IsUnique, string[] TakenFields)> CheckUserValidityAsync(string? username = null, string? email = null)
        {
            return await _unitOfWork.Users.CheckUserValidityAsync(username, email);
        }

        public async Task<UserResponseDTO> UpdateUserAsync(UserUpdateDto request)
        {
            var user = await _unitOfWork.Users.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
            }

            if (!string.IsNullOrEmpty(request.UserName) && !user.UserName.Equals(request.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var (isUnique, takenFields) = await CheckUserValidityAsync(username: request.UserName);
                if (!isUnique)
                {
                    throw new InvalidOperationException("Username is already taken.");
                }
            }

            var userProfile = await _profileService.GetProfileByUserIdAsync(request.UserId);
            if (userProfile == null)
            {
                throw new KeyNotFoundException($"Profile for user ID {request.UserId} not found.");
            }

            if (request.Image != null)
            {
                try
                {
                    userProfile.ImageUrl = await _fileService.UploadFileAsync(request.Image, "ProfileImages");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to upload file.", ex);
                }
            }

            if (!string.IsNullOrEmpty(request.UserName))
            {
                user.UserName = request.UserName;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                userProfile.Name = request.Name;
            }

            var profileRequest = new ProfileRequestDTO
            {
                UserId = userProfile.UserId,
                Email = userProfile.Email,
                Name = userProfile.Name,
                ImageUrl = userProfile.ImageUrl,
                UserName = user.UserName
            };

            await _profileService.UpdateAsync(userProfile.Id , profileRequest);

            await _unitOfWork.Users.UpdateUserAsync(user);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var newRefreshToken = await _tokenService.RefreshAccessToken(refreshToken);
                if (string.IsNullOrEmpty(newRefreshToken))
                {
                    throw new SecurityTokenException("Invalid or expired refresh token.");
                }

                var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(newRefreshToken);
                if (user == null)
                {
                    throw new SecurityTokenException("No user found with the provided refresh token.");
                }

                var accessToken = await _tokenService.GenerateAccessToken(user);
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new InvalidOperationException("Failed to generate access token.");
                }

                return new LoginResponseDTO
                {
                    User = _mapper.Map<UserResponseDTO>(user),
                    Token = accessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (SecurityTokenException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<LoginResponseDTO> LoginUserAsync(LoginRequestDTO request)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            if (!user.EmailConfirmed)
            {
                throw new InvalidOperationException("Email not confirmed. Please verify your email before logging in.");
            }

            var checkPassword = await _unitOfWork.Users.CheckPasswordSignInAsync(user, request.Password);
            if (!checkPassword)
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            var roles = await _unitOfWork.Users.GetRolesAsync(user);
            var rolesString = string.Join(",", roles);

            var userDto = _mapper.Map<UserResponseDTO>(user);
            userDto.Roles = roles.ToList();

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateUserAsync(user);

            return new LoginResponseDTO
            {
                User = userDto,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }


        public TimeSpan? CanRequestPasswordResetAsync(string email, DateTime? LastPasswordResetRequested)
        {
            var lastResetRequestTime = LastPasswordResetRequested;

            if (lastResetRequestTime == DateTime.MinValue)
                return null;

            var allowedInterval = TimeSpan.FromMinutes(15);

            var timeDifference = DateTime.UtcNow - lastResetRequestTime;

            if (timeDifference < allowedInterval)
            {
                return allowedInterval - timeDifference;
            }

            return null;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string email)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with this email not found :  {email}");
            }
            return await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<ApiResponse> SendVerificationEmailAsync(string email, string token)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            if (user.EmailConfirmed)
            {
                return new ApiResponse(400, "Email is already confirmed.");
            }

            if (string.IsNullOrEmpty(token))
            {
                return new ApiResponse(400, "Failed to generate email verification token.");
            }

            var cooldownMinutes = 1440;
            if (user.LastVerificationEmailSent.HasValue)
            {
                var lastSent = user.LastVerificationEmailSent.Value;
                var nextAllowedTime = lastSent.AddMinutes(cooldownMinutes);
                var timeRemaining = (int)(nextAllowedTime - DateTime.UtcNow).TotalSeconds;

                if (timeRemaining > 0)
                {
                    return new ApiResponse(429, $"Please wait {timeRemaining} seconds before requesting another verification email.");
                }
            }

            var confirmationUrl = token;
            var subject = "Email Verification";
            var placeholders = new Dictionary<string, string>
    {
        { "{CONFIRMATION_URL}", confirmationUrl }
    };

            try
            {
                await _emailService.SendMailAsync(user.Email, subject, "EmailVerificationTemplate.html", placeholders);

                user.LastVerificationEmailSent = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateUserAsync(user);

                return new ApiResponse(200, "Verification email has been sent.");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, "Failed to send verification email. Please try again later.");
            }
        }


        public async Task<ApiResponse> VerifyEmailAsync(string email, string token)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse(400, "User not found.");
            }

            if (user.EmailConfirmed)
            {
                return new ApiResponse(400, "Email is already confirmed.");
            }

            var result = await _unitOfWork.Users.ConfirmEmailAsync(user, token);
            if (!result)
            {
                return new ApiResponse(400, "Invalid or expired verification token.");
            }

            user.EmailConfirmed = true;
            await _unitOfWork.CompleteAsync();

            return new ApiResponse(200, "Email verified successfully.");
        }

    }
}
