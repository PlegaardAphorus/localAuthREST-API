using LocalAuthREST_API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserDB>(opt => opt.UseInMemoryDatabase("UserList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthorization();

byte[] key = Encoding.ASCII.GetBytes("Very_Secret_Secret_To_Secure_The_API_And_The_Data_Of_The_Users");
string issuer = "https://localhost:7031/:";
string audience = "https://localhost:7031/";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login/", async (User user, UserDB db) =>
{
    string username = user.Username;
    string password = user.Password;

    if (username == null || password == null)
    {
        return Results.Conflict("Sie müssen ein Benutzername und Passwort angeben.");
    }

    string hashedPassword;
    using (HashAlgorithm hash = SHA256.Create())
    {
        byte[] hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        hashedPassword = sb.ToString();
    }

    User authenticatedUser = await db.users.FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword);

    if (authenticatedUser != null)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        { 
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, authenticatedUser.Username),
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);

        return Results.Ok(new { Token = tokenString });
    }

    return authenticatedUser != null ? Results.Ok(authenticatedUser) : Results.NotFound();
});

app.MapPost("/api/auth/register", async (User User, UserDB db) =>
{

    if (await db.users.AnyAsync(u => u.Username == User.Username))
    {
        return Results.Conflict("Ein Benutzer mit diesem Benutzernamen existiert bereits.");
    }

    using (HashAlgorithm hash = SHA256.Create())
    {
        byte[] hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(User.Password));
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        User.Password = sb.ToString();
    }

    db.users.Add(User);
    await db.SaveChangesAsync();

    return Results.Created($"/user/{User.Id}", User);
});

app.MapGet("/api/user", async (UserDB db) => await db.users.ToListAsync()).RequireAuthorization();

app.MapGet("/api/user/me", async (ClaimsPrincipal user, UserDB db) =>
{
    int userID = Convert.ToInt32(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    User currentUser = await db.users.FirstOrDefaultAsync(u => u.Id == userID);

    return currentUser != null ? Results.Ok(currentUser) : Results.Unauthorized();

}).RequireAuthorization();

app.Run();