using LocalAuthREST_API.controllers;
using System.Security.Claims;

namespace LocalAuthREST_API.routes
{
    public static class UsersRoutes
    {
        public static void AllUsers(WebApplication app)
        {
            app.MapGet("/api/user", async (UserDB db) => UserController.GetAllUser(db)).RequireAuthorization();
        }

        public static void SelfUser(WebApplication app)
        {
            app.MapGet("/api/user/me", async (UserDB db, ClaimsPrincipal user) => UserController.GetMe(db, user)).RequireAuthorization();
        }
    }
}