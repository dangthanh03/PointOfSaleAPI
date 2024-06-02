using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoAnAPI.Repository
{
    public class UserAuthenticateRepository : IUserAuthenticateRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ICartRepository cartRepository;

        public UserAuthenticateRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ICartRepository cartRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.cartRepository = cartRepository;
        }

        public async Task<Result<string>> RegisterAsync(RegistrationModel model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return Result<string>.Fail("User already exists");
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return Result<string>.Fail("User creation fail");
            }
            var cartResult = await cartRepository.CreateCartAsync(user.Id);
            if (!cartResult.IsSuccess)
            {
                // Xử lý lỗi tạo giỏ hàng
                return Result<string>.Fail($"Error creating cart: {cartResult.Message}");
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
                await roleManager.CreateAsync(new IdentityRole(model.Role));

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                await userManager.AddToRoleAsync(user, model.Role);
            }

            return Result<string>.Success("You have registered successfully");
        }

        public async Task<Result<UserProfileVm>> LoginAsync(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Result<UserProfileVm>.Fail("Invalid username");
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Result<UserProfileVm>.Fail("Invalid Password");
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var userProfile = new UserProfileVm
                {
                    id = user.Id,
                    Name = user.UserName,
                };

                return Result<UserProfileVm>.Success(userProfile);
            }
            else if (signInResult.IsLockedOut)
            {
                return Result<UserProfileVm>.Fail("User is locked out");
            }
            else
            {
                return Result<UserProfileVm>.Fail("Error on logging in");
            }
        }

        public async Task<Result<string>> LogoutAsync()
        {
            await signInManager.SignOutAsync();
            return Result<string>.Success("Logged out successfully");
        }

        public async Task<Result<List<string>>> GetAvailableRoles()
        {
            try
            {
                var roles = await roleManager.Roles.ToListAsync();
                return Result<List<string>>.Success(roles.Select(r => r.Name).ToList());
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return Result<List<string>>.Fail("An error occurred while retrieving available roles.");
            }
        }
        public async Task<Result<ApplicationUser>> GetCurrentUserInfoAsync(ClaimsPrincipal userPrincipal)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (userPrincipal == null || !userPrincipal.Identity.IsAuthenticated)
            {
                return Result<ApplicationUser>.Fail("User is not authenticated");
            }

            // Lấy thông tin người dùng từ UserManager
            var user = await userManager.GetUserAsync(userPrincipal);
            if (user == null)
            {
                return Result<ApplicationUser>.Fail("User not found");
            }

            // Trả về thông tin người dùng
            return Result<ApplicationUser>.Success(user);
        }
        public async Task<Result<int>> CheckUserRoleAsync(ApplicationUser user)
        {
            if (!await userManager.IsInRoleAsync(user, "user"))
            {
                return Result<int>.Fail("Not user");
            }
            return Result<int>.Success(1);
        }
        public async Task<Result<int>> CheckAdminRoleAsync(ApplicationUser user)
        {
            if (!await userManager.IsInRoleAsync(user, "admin"))
            {
                return Result<int>.Fail("Not admin");
            }
            return Result<int>.Success(1);
        }
    }
}
