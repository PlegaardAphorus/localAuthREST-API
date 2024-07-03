using LocalAuthREST_API.models;
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

        public static async Task<String> CreateToken(UserDB db, User user, IConfigurationRoot config)
        {
            string username = user.Username;
            string password = user.Password;

            if (username == null || password == null)
            {
                throw new ArgumentNullException("Sie müssen ein Benutzername und ein Passwort angeben.");
            }
            else
            {
                string hashedPassword;
                hashedPassword = HashPassword.PasswordHash(password);

                User authenticatedUser = await db.users.FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword);

                if (authenticatedUser != null)
                {
                    string token = JwtToken.CreateToken(authenticatedUser, config);
                    Console.WriteLine(token);
                    return token;
                }
                else
                {
                    throw new KeyNotFoundException("Es wurde kein Benutzer mit den angegebenen Daten gefunden.");
                }
            }
        }

        public static IResult LoginUser(UserDB db, User user, IConfigurationRoot config)
        {
            string token;
            try
            {
                token = CreateToken(db, user, config).Result;
            }
            catch(Exception ex)
            {
                if (ex is ArgumentNullException)
                {
                    return Results.Conflict(ex.Message);
                }
                else if (ex is KeyNotFoundException)
                {
                    return Results.NotFound(ex.Message);
                }
                else
                {
                    return Results.BadRequest("Es ist ein unerwarteter Fehler aufgetreten bitte versuche es später erneut.");
                }
            } 
            return Results.Ok(new { Token = token });
        }
    }
}