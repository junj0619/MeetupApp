
using System.Collections.Generic;
using System.Linq;
using MeetupApp.API.Models;
using Newtonsoft.Json;

namespace MeetupApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {   /*  
             To create some test users into database 
                1) Read seed data from json file
                2) Create password Salt and Hash
                3) lowercase username
                4) save to users context
            */
            if (!context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach (var user in users)
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.Users.Add(user);
                }

                context.SaveChanges();
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