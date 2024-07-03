using LocalAuthREST_API.controllers;
using LocalAuthREST_API.models;

namespace LocalAuthREST_API.routes
{
    public static class AuthRoutes
    {
        public static void Register(WebApplication app)
        {
            app.MapPost("/api/auth/register", async (User user, UserDB db) => await AuthController.RegisterUser(db, user));
        }

        public static void Login(WebApplication app, IConfigurationRoot config)
        {
            app.MapPost("/api/auth/login/", async (User user, UserDB db) => AuthController.LoginUser(db, user, config));
        }

    }
}
