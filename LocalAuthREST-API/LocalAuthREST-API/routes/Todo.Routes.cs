using LocalAuthREST_API.controllers;
using LocalAuthREST_API.models;
using Microsoft.AspNetCore.Mvc;

namespace LocalAuthREST_API.routes
{
    public class TodoRoutes
    {
        public static void GetAllTodos(WebApplication app)
        {
            app.MapGet("/api/todo/list", async ([FromServices] TodoDB db) => TodoController.GetAllTodos(db));
        }

        public static void GetAllCompletedTodos(WebApplication app)
        {
            app.MapGet("/api/todo/list/complete", async ([FromServices] TodoDB db) => TodoController.GetAllCompletedTodos(db)).RequireAuthorization();
        }

        public static void GetTodoFromID(WebApplication app)
        {
            app.MapGet("/api/todo/list/{id}", async (int id, [FromServices] TodoDB db) => TodoController.GetTodoFromID(db, id)).RequireAuthorization();
        }

        public static void AddTodo(WebApplication app)
        {
            app.MapPost("/api/todo/add", async (Todo todo, [FromServices] TodoDB db) => TodoController.AddTodo(todo, db)).RequireAuthorization();
        }

        public static void DeleteTodo(WebApplication app)
        {
            app.MapDelete("/api/todo/delete/{id}", async (int id, [FromServices] TodoDB db) => TodoController.DeleteTodo(id, db)).RequireAuthorization();
        }
    }
}
