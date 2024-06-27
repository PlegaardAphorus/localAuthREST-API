using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

        public DbSet<Todo> todos => Set<Todo>();
    }
}
