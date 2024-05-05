using API.FurniteStore.Shared;
using API.FurniteStore.Shared.Common;
using API.FurniteStore.Shared.DTOs;
using API.Store.Configuration;
using API.StoreData;
using API.StoreShared.Auth;
using API.StoreShared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace API.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly IEmailSender _emailSender;
        private readonly ApiStoreDbContext _dbContext;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(UserManager<IdentityUser> userManager, IOptions<JwtConfig> jwtConfig, IEmailSender emailSender, 
                                        ApiStoreDbContext dbContext, TokenValidationParameters tokenValidationParameters,
                                        ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _tokenValidationParameters = tokenValidationParameters;
            _logger = logger;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto request)
        {
            _logger.LogWarning("A user is trying to register");
            if (!ModelState.IsValid) return BadRequest();

            //Verify if email exists
            var emailExists = await _userManager.FindByEmailAsync(request.Email);

            if (emailExists != null)
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email already exists"
                    }
                });

            //Create user
            var user = new IdentityUser()
            {
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = false
            };

            var isCreated = await _userManager.CreateAsync(user, request.Password);

            if (isCreated.Succeeded)
            {
                await SendVerificationEmail(user);

                return Ok(new AuthResult()
                {
                    Result = true
                });
            }
            else
            {
                var errors = new List<string>();
                foreach (var err in isCreated.Errors)
                    errors.Add(err.Description);

                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = errors
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
        {
            //Valido si se mandaron los campos requeridos
            if (!ModelState.IsValid) return BadRequest();
            //Busco el usuario y lo guardo en una variable
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if(existingUser == null) return BadRequest(new AuthResult()
            {
                Result = true,
                Errors = new List<string>() { "Invalid payload" }
            });

            if (!existingUser.EmailConfirmed)
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Email needs to be confirmed." },
                    Result = false
                });

            var checkPassword = await _userManager.CheckPasswordAsync(existingUser, request.Password);
            if (!checkPassword) return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>() { "Invalid payload" }
            });

            var token = GenerateTokenAsync(existingUser);
            return Ok(token);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid email confirmation url" },
                    Result = false
                });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Unable to load user with Id '{userId}'.");

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            var status = result.Succeeded ? "Thank you for confirming your email."
                : "There has been an error confirming your email.";
            return Ok(status);
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid parameters" },
                    Result = false
                });

            var results = VerifyAndGenerateTokenAsync(tokenRequest);

            if (results == null)
                return BadRequest(new AuthResult
                {
                    Errors = new List<string> { "Invalid token" },
                    Result = false
                });

            return Ok(results);
        }
        private async Task<AuthResult> GenerateTokenAsync(IdentityUser user)
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

            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomGenerator.GenerateRandomString(23),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.Id,
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResult
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Result = true
            };
        }


        private async Task<AuthResult> VerifyAndGenerateTokenAsync(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;

                var tokenBeingVerified = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);

                    if (!result || tokenBeingVerified == null)
                        throw new Exception("Invalid Token");
                }

                var utcExpiryDate = long.Parse(tokenBeingVerified.Claims.
                        FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;
                if (expiryDate < DateTime.UtcNow)
                    throw new Exception("Token Expired");

                var storedToken = await _dbContext.RefreshTokens.
                        FirstOrDefaultAsync(t => t.Token == tokenRequest.RefreshToken);
                if (storedToken == null)
                    throw new Exception("Invalid Token");

                if (storedToken.IsUsed || storedToken.IsRevoked)
                    throw new Exception("Invalid Token");

                var jti = tokenBeingVerified.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

                if (jti != storedToken.JwtId)
                    throw new Exception("Invalid Token");

                if (storedToken.ExpiryDate < DateTime.UtcNow)
                    throw new Exception("Token Expired");

                storedToken.IsUsed = true;
                _dbContext.RefreshTokens.Update(storedToken);
                await _dbContext.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);

                return await GenerateTokenAsync(dbUser);
            }
            catch (Exception e)
            {
                var message = e.Message == "Invalid Token" || e.Message == "Token Expired"
                    ? e.Message
                    : "Internal Server Error";
                return new AuthResult()
                {
                    Result = false,
                    Errors = new List<string> { message }
                };
            }
        }

        private async Task SendVerificationEmail(IdentityUser user)
        {
            var verificationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            verificationCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(verificationCode));

            var callbackUrl = $@"{Request.Scheme}://{Request.Host}{Url.Action("ConfirmEmail", controller: "Authentication",
                                    new { userId = user.Id, code = verificationCode })}";

            var emailBody = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", emailBody);
        }
    }
}
