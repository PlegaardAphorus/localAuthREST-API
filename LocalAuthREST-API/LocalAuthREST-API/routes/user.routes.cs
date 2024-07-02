using LocalAuthREST_API.controllers;
using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LocalAuthREST_API.routes
{
    public static class usersRoutes
    {
        public static void allUsers(WebApplication app)
        {
            app.MapGet("/api/user", async (UserDB db) => await db.users.ToListAsync()).RequireAuthorization();

        }

        public static void selfUser(WebApplication app)
        {
            app.MapGet("/api/user/me", async (ClaimsPrincipal user, UserDB db) =>
            {
                int userID = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                User currentUser = await db.users.FirstOrDefaultAsync(u => u.Id == userID);

                return currentUser != null ? Results.Ok(currentUser) : Results.Unauthorized();

            }).RequireAuthorization();
        }
    }
}