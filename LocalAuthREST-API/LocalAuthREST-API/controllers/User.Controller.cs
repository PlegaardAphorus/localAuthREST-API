using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LocalAuthREST_API.controllers
{
    public class UserController
    {
        public static IResult GetMe(UserDB db, ClaimsPrincipal user)
        {
            int userID = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var currentUser = db.users.FirstOrDefaultAsync(u => u.Id == userID);
            return currentUser != null ? Results.Ok(currentUser) : Results.Unauthorized();
        }

        public static Task<List<User>> GetAllUser(UserDB db)
        {
            return db.users.ToListAsync();
        }
    }
}
