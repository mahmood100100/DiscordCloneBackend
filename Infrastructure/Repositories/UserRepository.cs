using DiscordCloneBackend.Application.DTOs;
using DiscordCloneBackend.Core.Entities;
using DiscordCloneBackend.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordCloneBackend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<LocalUser> userManager;
        private readonly SignInManager<LocalUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRepository(UserManager<LocalUser> userManager, SignInManager<LocalUser> signInManager , RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> AddToRoleAsync(LocalUser user, string role)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(role))
                throw new ArgumentException("Role cannot be null or empty", nameof(role));

            return await userManager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateUserAsync(LocalUser user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return await userManager.CreateAsync(user, password);
        }

        public async Task<IEnumerable<LocalUser>> GetAllUsersAsync()
        {
            return await userManager.Users
                .Include(u => u.Profile)
                .ToListAsync();
        }


        public async Task<LocalUser> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await userManager.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<LocalUser> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty", nameof(id));

            return await userManager.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(bool IsUnique, string[] TakenFields)> CheckUserValidityAsync(string? username = null, string? email = null)
        {
            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                throw new ArgumentException("At least one of username or email must be provided.");

            List<string> takenFields = new List<string>();

            if (!string.IsNullOrEmpty(username) && await userManager.Users.AnyAsync(user => user.UserName.ToLower() == username.ToLower()))
                takenFields.Add("Username");

            if (!string.IsNullOrEmpty(email) && await userManager.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower()))
                takenFields.Add("Email");

            return (takenFields.Count == 0, takenFields.ToArray());
        }



        public async Task<bool> CheckPasswordSignInAsync(LocalUser user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            return result.Succeeded;
        }

        public async Task<IList<string>> GetRolesAsync(LocalUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await userManager.GetRolesAsync(user);
        }

        public async Task<IList<string>> GetRolesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            return await userManager.GetRolesAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(LocalUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(LocalUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(LocalUser user, string token, string newPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("New password cannot be null or empty", nameof(newPassword));

            return await userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> ChangePasswordAsync(LocalUser user, string currentPassword, string newPassword)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(currentPassword))
                throw new ArgumentException("Current password cannot be null or empty", nameof(currentPassword));
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("New password cannot be null or empty", nameof(newPassword));

            return await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException($"User with Id {userId} not found.");

            Console.WriteLine(user);
            return await userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(LocalUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return await userManager.UpdateAsync(user);
        }

        public async Task RemoveFromAllRolesAsync(LocalUser user)
        {
            var currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    throw new InvalidOperationException("Failed to remove user from roles.");
                }
            }
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            return role != null;
        }

        public async Task<LocalUser> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var user = await userManager.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            return user;
        }

        public async Task<bool> ConfirmEmailAsync(LocalUser user, string token)
        {
            var result = await userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }
    }
}