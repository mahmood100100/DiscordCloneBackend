using DiscordCloneBackend.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DiscordCloneBackend.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider , AppDbContext appDbContext)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<LocalUser>>();

            await SeedRoles(RoleManager);
            await SeedAdminUser(UserManager , appDbContext);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };
            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<LocalUser> userManager, AppDbContext appDbContext)
        {
            if (await userManager.FindByEmailAsync("admin@discord.com") == null)
            {
                var user = new LocalUser
                {
                    UserName = "Mahmoud",
                    Email = "admin@discord.com"
                };

                var result = await userManager.CreateAsync(user, "MahmoudAdmin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");

                    // Create a Profile for the Admin
                    var profile = new Profile
                    {
                        UserId = user.Id,
                        Name = "Admin",
                        Email = user.Email,
                        UserName = user.UserName,
                        ImageUrl = "https://imgs.search.brave.com/vim1XWlkOiBZ3rb1R2B855LkTE_q1rp1yfOXEHj098U/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly90NC5m/dGNkbi5uZXQvanBn/LzAxLzM3Lzk4Lzcx/LzM2MF9GXzEzNzk4/NzE1N191cEp0eUxq/d3lnVGxBTzBIQTVu/TnNIclR1RWcxdlAw/Zy5qcGc"
                    };

                    appDbContext.Profiles.Add(profile);
                    await appDbContext.SaveChangesAsync();
                }
            }
        }
    }
}
