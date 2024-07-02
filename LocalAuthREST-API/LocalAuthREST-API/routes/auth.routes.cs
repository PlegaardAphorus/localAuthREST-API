using LocalAuthREST_API.controllers;
using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API.routes
{
    public static class authRoutes
    {
        public static void register(WebApplication app)
        { 
            app.MapPost("/api/auth/register", async (User User, UserDB db) =>
            {

                if (await db.users.AnyAsync(u => u.Username == User.Username))
                {
                    return Results.Conflict("Ein Benutzer mit diesem Benutzernamen existiert bereits.");
                }

                User.Password = HashPassword.hash(User.Password);

                db.users.Add(User);
                await db.SaveChangesAsync();

                return Results.Created($"/user/{User.Id}", User);
            });
        }

        public static void login(WebApplication app, IConfigurationRoot config)
        {
            app.MapPost("/api/auth/login/", async (User user, UserDB db) =>
            {
                string username = user.Username;
                string password = user.Password;

                if (username == null || password == null)
                {
                    return Results.Conflict("Sie müssen ein Benutzername und Passwort angeben.");
                }

                string hashedPassword;
                hashedPassword = HashPassword.hash(password);

                User authenticatedUser = await db.users.FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword);

                if (authenticatedUser != null)
                {
                    return Results.Ok(new { Token = JwtToken.createToken(authenticatedUser, config) });
                }

                return authenticatedUser != null ? Results.Ok(authenticatedUser) : Results.NotFound();
            });
        }

    }
}
