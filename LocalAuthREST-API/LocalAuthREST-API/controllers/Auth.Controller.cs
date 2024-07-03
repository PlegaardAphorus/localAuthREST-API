﻿using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API.controllers
{
    public class AuthController
    {
        public static async Task<IResult> RegisterUser(UserDB db, User user)
        {

            if (await db.users.AnyAsync(u => u.Username == user.Username))
            {
                return Results.Conflict("Ein Benutzer mit diesem Benutzernamen existiert bereits.");
            }

            user.Password = HashPassword.PasswordHash(user.Password);

            db.users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created("/user/me", user);
        }

        public static async Task<IResult> LoginUser(UserDB db, User user, IConfigurationRoot config)
        {
            string username = user.Username;
            string password = user.Password;

            if (username == null || password == null)
            {
                return Results.Conflict("Sie müssen ein Benutzername und Passwort angeben.");
            }

            string hashedPassword;
            hashedPassword = HashPassword.PasswordHash(password);

            User authenticatedUser = await db.users.FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword);

            if (authenticatedUser != null)
            {
                Console.WriteLine(JwtToken.CreateToken(authenticatedUser, config));
                return Results.Ok(new { Token = JwtToken.CreateToken(authenticatedUser, config) });
            }
            else
            {
                 return Results.NotFound();
            }
        }
    }
}