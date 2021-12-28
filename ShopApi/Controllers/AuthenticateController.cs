using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using ShopApi.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ShopUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ShopUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route(nameof(SignIn))]
        public async Task<IResult> SignIn([FromBody] SignInDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                JwtSecurityToken token = GenerateToken(user, userRoles);

                return Results.Ok(new
                {
                    userId = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    role = userRoles[0]
                });
            }

            return Results.Unauthorized();
        }



        [HttpPost]
        [Route(nameof(SignUp))]
        public async Task<IResult> SignUp([FromBody] SignUpDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return Results.Conflict(new Response { Status = ResponseStatus.Error, Message = "User already exists!" });

            ShopUser user = new ShopUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(new Response { Status = ResponseStatus.Error, Message = "User creation failed! Please check user details and try again." });
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            JwtSecurityToken token = GenerateToken(user, userRoles);

            return Results.Ok(new
            {
                userId = user.Id,
                username = user.UserName,
                email = user.Email,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                role = userRoles[0]
            });
        }

        [HttpPost]
        [Route(nameof(RegisterAdmin))]
        public async Task<IResult> RegisterAdmin([FromBody] SignUpDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);

            if (userExists != null)
                return Results.Conflict(new Response { Status = ResponseStatus.Error, Message = "User already exists!" });

            ShopUser user = new ShopUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return Results.BadRequest(new Response { Status = ResponseStatus.Error, Message = "User creation failed! Please check user details and try again." });
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Results.Ok(new Response { Status = ResponseStatus.Success, Message = "User created successfully!" });
        }

        [Authorize]
        [HttpGet]
        [Route(nameof(SignOut))]
        public async Task<IResult> SignOut()
        {
            var jwt = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var username = token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Results.NotFound(new Response { Status = ResponseStatus.Error, Message = "User not found!" });
            }            

            return Results.Ok(new Response { Status = ResponseStatus.Success, Message = "User signed out successfully!" });
        }


        private JwtSecurityToken GenerateToken(ShopUser user, IList<string> userRoles)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
