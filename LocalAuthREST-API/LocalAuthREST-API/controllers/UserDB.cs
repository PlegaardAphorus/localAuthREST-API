using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API.controllers
{
    public class UserDB : DbContext
    {
        public UserDB(DbContextOptions<UserDB> options) : base(options) { }

        public DbSet<User> users => Set<User>();
    }
}
