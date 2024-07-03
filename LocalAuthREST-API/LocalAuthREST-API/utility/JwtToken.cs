using LocalAuthREST_API.models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LocalAuthREST_API.controllers
{
    public class JwtToken
    {
        public static string CreateToken(User authenticatedUser, IConfigurationRoot config)
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
                Issuer = Issuer(config),
                Audience = Audience(config),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key(config)), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public static string Issuer(IConfigurationRoot config)
        {
            return config["JWT_ISSUER"];
        }

        public static string Audience(IConfigurationRoot config)
        {
            return config["JWT_AUDIENCE"];
        }
        
        public static byte[] Key(IConfigurationRoot config)
        {
            return Encoding.ASCII.GetBytes(config["JWT_SECRET_TOKEN"]);
        }
    }
}
