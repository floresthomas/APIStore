using API.Store.Configuration;
using API.StoreShared.Auth;
using API.StoreShared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig; 

        public AuthenticationController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto request)
        {
            //Valido si mandan todos los campos requeridos
            if (!ModelState.IsValid) return BadRequest();
            //Busco si existe el email
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            //Si el email ya existe, entonces arrojo un error
            if(emailExists != null)
            {
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>() { "Email already exists" }
                });
            }
            // Creamos una instancia de IdentityUser y asignamos los valores del objeto request. 
            var user = new IdentityUser()
            {
                Email = request.Email,
                UserName = request.UserName,
            };
            var isCreated = await _userManager.CreateAsync(user);
            //Si la creacion fue exitosa, entonces devuelvo una respuesta con el result y el token
            if (isCreated.Succeeded) 
            {
                var token = GenerateTokenAsync(user);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                }); 
            }
            else
            {
                var errors = new List<string>();
                foreach(var err in errors)
                {
                    errors.Add(err);
                }
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = errors
                });
            }

        }
        private string GenerateTokenAsync(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                })),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
