using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoAnAPI.IRepository
{
    public interface IUserAuthenticateRepository
    {
        Task<Result<int>> CheckUserRoleAsync(ApplicationUser user);
        Task<Result<int>> CheckAdminRoleAsync(ApplicationUser user);
        Task<Result<UserProfileVm>> LoginAsync(LoginModel model);
        Task<Result<string>> LogoutAsync();
        Task<Result<string>> RegisterAsync(RegistrationModel model);
        Task<Result<List<string>>> GetAvailableRoles();
        Task<Result<ApplicationUser>> GetCurrentUserInfoAsync(ClaimsPrincipal userPrincipal);
    }
}
