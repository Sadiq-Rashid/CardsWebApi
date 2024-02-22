using Cards.Models;
using System.Security.Claims;

namespace Cards.Services
{
    public interface IAuthService
    {
        Task<bool> Login(LoginUser user);
        Task<string> getRoleByUserId(Guid userId);
        string GenerateTokenString(LoginUser user);
        Task<bool> RegisterUser(LoginUser user);
        Task<Guid> GetLoggedInUserIdAsync(ClaimsPrincipal user);
        Task<bool> assignRole(string userId, string roleName);
    }

}
