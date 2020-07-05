using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Persistance
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> _userManager, RoleManager<Role> _roleManager)
        {
            if (!_userManager.Users.Any())
            {
                // Create Roles
                var roles = new List<Role>
                {
                    new Role { Name="Member" },
                    new Role { Name="Admin" },
                    new Role { Name="Moderator" },
                    new Role { Name="Support" },
                    new Role { Name="CallCenter" },
                };

                // Add Roles
                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                // Create admin user
                var adminUser = new User
                {
                    UserName = "Admin"
                };

                // Add user and assign admin user role
                var result = _userManager.CreateAsync(adminUser, "Password@Corona").Result;
                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin", "Support", "CallCenter" }).Wait();
                }
            }
            
        }
    }
}
