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
using PnLReporter.Models;
using PnLReporter.EnumInfo;
using PnLReporter.Service;
using PnLReporter.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JWTAuthentication.Controllers
{
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private readonly PLSystemContext _context;
        private IParticipantService _service;

        public LoginController(IConfiguration config, PLSystemContext context)
        {
            _config = config;
            _context = context;
            _service = new ParticipantService(new ParticipantRepository(_context));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Login()
        {
            IActionResult response = Unauthorized();
            
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(Request.Headers[HeaderNames.Authorization]);
            string uid = decodedToken.Uid;
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);

            var user = _service.FindByUsername(userRecord.Email);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString, participant = user });
            }

            return response;
        }

        private string GenerateJSONWebToken(UserModel participant)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, participant.Id + ""),
                    new Claim(ClaimTypes.Email, participant.Username),
                    new Claim(ClaimTypes.Role, ParticipantsRoleEnum.GetRole(participant.Role))
                }),
                Audience = _config["Jwt:Issuer"],
                Issuer = _config["Jwt:Issuer"],
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        //[HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Route("/logininfo")]
        //public IActionResult GetLoginInfo()
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    string name = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    string role = identity.FindFirst(ClaimTypes.Role).Value;
        //    return Ok(new { name = name, role = role});
        //}
    }
}