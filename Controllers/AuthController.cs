using Cards.Models;
using Cards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cards.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;
      

        public AuthController(IAuthService authService, UserManager<IdentityUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;

        }


        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(LoginUser user)
        {
            if(await _authService.RegisterUser(user))
            {
                return Ok("Successfully Registered!");
            }
            return BadRequest("Something went wrong");
            

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            if (await _authService.Login(user))
            {
                var tokenString = _authService.GenerateTokenString(user);
                return Ok(tokenString);
            }

            return BadRequest();
        }

        [HttpPost("AssignRole")]
        [Authorize]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            
            await _authService.assignRole(userId, roleName);
            // Handle success
            return Ok();
        }


    }
}
