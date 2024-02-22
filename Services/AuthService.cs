using Cards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cards.Services
{
    public class AuthService : IAuthService

    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
     

        public AuthService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {

            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        public async Task<Guid> GetLoggedInUserIdAsync(ClaimsPrincipal user)
        {
            var userEmail = user.FindFirstValue(ClaimTypes.Email);
            var loggedInUser = await _userManager.FindByEmailAsync(userEmail);

            if (loggedInUser != null)
            {
                return Guid.Parse(loggedInUser.Id);
            }
            throw new InvalidOperationException("Logged-in user not found");
        }


        public async Task<bool> RegisterUser(LoginUser user)
        {
            var idenityUser = new IdentityUser
            {
                UserName = user.UserName,
                Email = user.UserName
            };

            var result = await _userManager.CreateAsync(idenityUser, user.Password);
            if (!await _roleManager.RoleExistsAsync(UserRole.Admin));
                await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRole.User)) ;
            await _roleManager.CreateAsync(new IdentityRole(UserRole.User));
            if (await _roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await _userManager.AddToRoleAsync(idenityUser, UserRole.Admin);
            }

            return result.Succeeded;
        }

        public async Task<bool> Login(LoginUser user)
          {
              var identityUser = await _userManager.FindByEmailAsync(user.UserName);
              if (identityUser is null)
              {
                  return false;
              }
              return await _userManager.CheckPasswordAsync(identityUser, user.Password);
          }

    
        //Generate Token
        public string GenerateTokenString(LoginUser user)
        {
            var identityUser = _userManager.FindByEmailAsync(user.UserName).Result;

            if (identityUser == null)
            {
                // Handle the case when the user is not found.
                return null;
            }

            var userRoles = _userManager.GetRolesAsync(identityUser).Result;
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityToken = new JwtSecurityToken(
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(60),
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Audience").Value,
                signingCredentials: signingCred);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }


        public async Task<string> getRoleByUserId(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                // Handle the case where the user is not found
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Select the first role or apply your logic to choose one
            var userRole = roles.FirstOrDefault();

            return userRole;

        }

        public async Task<bool> assignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
    
            var loggedInUser = _userManager.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                // Handle user not found
                return false;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                // Handle role not found
                return false;
            }

            // Assign the user to the role
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return true;
            }

      
    }

       


}
