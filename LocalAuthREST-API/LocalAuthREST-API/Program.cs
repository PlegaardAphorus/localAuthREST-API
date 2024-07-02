using LocalAuthREST_API.controllers;
using LocalAuthREST_API.routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), @"config\.env"));
IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDB>(opt => opt.UseInMemoryDatabase("UserList"));
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
        ValidIssuer = JwtToken.issuer(config),
        ValidAudience = JwtToken.audience(config),
        IssuerSigningKey = new SymmetricSecurityKey(JwtToken.key(config))
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

authRoutes.register(app);
authRoutes.login(app, config);

usersRoutes.allUsers(app);
usersRoutes.selfUser(app);

app.Run();