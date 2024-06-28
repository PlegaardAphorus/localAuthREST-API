using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API
{
    public class UserDB : DbContext
    {
        public UserDB(DbContextOptions<UserDB> options) : base(options) { }

        public DbSet<User> users => Set<User>();
    }
}
