using LocalAuthREST_API.controllers;
using LocalAuthREST_API.routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), @"config\.env"));
IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDB>(opt => opt.UseInMemoryDatabase("UserList"));
builder.Services.AddDbContext<TodoDB>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtToken.Issuer(config),
        ValidAudience = JwtToken.Audience(config),
        IssuerSigningKey = new SymmetricSecurityKey(JwtToken.Key(config))
    };
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

AuthRoutes.Register(app);
AuthRoutes.Login(app, config);

UsersRoutes.AllUsers(app);
UsersRoutes.SelfUser(app);

TodoRoutes.GetAllTodos(app);
TodoRoutes.GetAllCompletedTodos(app);
TodoRoutes.GetTodoFromID(app);
TodoRoutes.AddTodo(app);
TodoRoutes.DeleteTodo(app);

app.Run();