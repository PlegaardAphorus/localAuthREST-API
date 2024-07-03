using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API.controllers
{
    public class TodoDB : DbContext
    {
        public TodoDB(DbContextOptions<TodoDB> options) : base(options) { }

        public DbSet<Todo> todos => Set<Todo>();
    }
}
