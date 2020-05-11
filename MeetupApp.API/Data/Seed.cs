
using System.Collections.Generic;
using System.Linq;
using MeetupApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MeetupApp.API.Data
{
    public class Seed
    {
        // public static void SeedUsers(DataContext context)
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {   /*  
             To create some test users into database 
                1) Read seed data from json file
                2) Create password Salt and Hash
                3) lowercase username
                4) save to users context
            */
            if (!userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                var roles = new List<Role>
                {
                    new Role{Name="Member"},
                    new Role{Name="Admin"},
                    new Role{Name="Moderator"},
                    new Role{Name="VIP"}
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                foreach (var user in users)
                {
                    user.Photos.SingleOrDefault().IsApproved = true;
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Member").Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                var result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }).Wait();
                }

            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            /* 
               We use SHA512 to generate random passwordSalt
               then we use this salt to hash given password 
            */
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }
    }
}