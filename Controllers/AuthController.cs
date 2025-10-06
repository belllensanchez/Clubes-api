using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrabajoProyecto.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrabajoProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IConfiguration _config; //iconf es un tipo de dato,

        public AuthController (IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Login")]

        public ActionResult<LoginResponse> Login([FromBody]LoginRequest req)
        {
            var valUser = _config["DemoUser:UserName"];
            var valPass = _config["DemoUser:Password"];

            if (req.UserName != valUser || req.Password != valPass)
            {
                return Unauthorized(new {message ="Credenciales incorrectas"});
            }

            var claims = new List<Claim>();
            {
                new Claim(ClaimTypes.Name, req.UserName);
                new Claim(ClaimTypes.Role, "Admin");
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims : claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds 
                );
            
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                

            return Ok(new LoginResponse { Token = jwt, ExpiresAtUtc = token.ValidTo});
        } 
    }
}
