using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirebaseAdmin.Auth;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login()
        {
            IActionResult response = Unauthorized();

            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(Request.Headers[HeaderNames.Authorization]);
            string uid = decodedToken.Uid;
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

            if (userRecord != null)
            {
                var tokenString = GenerateJSONWebToken(userRecord);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(UserRecord userInfo)
        {
            //var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Key"]));
            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Uid),
                    new Claim(ClaimTypes.Role, "admin")
                }),
                Audience = _config["Jwt:Issuer"],
                Issuer = _config["Jwt:Issuer"],
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}