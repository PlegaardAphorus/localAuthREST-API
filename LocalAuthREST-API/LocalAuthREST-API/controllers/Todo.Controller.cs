using LocalAuthREST_API.models;
using Microsoft.EntityFrameworkCore;

namespace LocalAuthREST_API.controllers
{
    public class TodoController
    {
        public static Task<List<Todo>> GetAllTodos(TodoDB db)
        {
            return db.todos.ToListAsync();
        }

        public static Task<List<Todo>> GetAllCompletedTodos(TodoDB db)
        {
            return db.todos.Where(t => t.Completed).ToListAsync();
        }

        public static Task<List<Todo>> GetTodoFromID(TodoDB db, int id)
        {
            return db.todos.Where(t => t.Id == id).ToListAsync();
        }

        public async static Task<IResult> AddTodo(Todo todo, TodoDB db)
        {
            db.todos.Add(todo);
            await db.SaveChangesAsync();

            return Results.Created($"/todo/list/{todo.Id}", todo);
        }

        public async static Task<IResult> DeleteTodo(int id, TodoDB db)
        {
            if (await db.todos.FindAsync(id) is Todo todo)
            {
                db.todos.Remove(todo);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }

            return Results.NotFound();
        }
    }
}
