using DoAnAPI.IRepository;
using DoAnAPI.Models.Domain;
using DoAnAPI.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthenticateController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> userManager;
        private IUserAuthenticateRepository authService;
        public UserAuthenticateController(IUserAuthenticateRepository authService, UserManager<ApplicationUser> _userManager)
        {
            this.userManager = _userManager;
            this.authService = authService;
        }

        [HttpGet("RegisterTest")]
        public async Task<IActionResult> Register11()
        {
            var model = new RegistrationModel
            {

                Email = "thanh@gmail.com",
                Username = "thanh",
                Name = "Jake",
                Password = "Thanh@123",
                PasswordConfirm = "Thanh@123",
                Role = "User"

            };

            var result = await authService.RegisterAsync(model);
            return Ok(result.Data);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(model);
            }

            var result = await authService.LoginAsync(model);

            if (result.IsSuccess)
            {

                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);

            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result= await authService.LogoutAsync();
            return Ok(result.Data);
         
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
               var roles = await authService.GetAvailableRoles();
                model.AvailableRoles = roles.Data;
               
                return BadRequest(model);
            }

            var result = await authService.RegisterAsync(model);

            if (result.IsSuccess)
            {

                return Ok(result.Data);
            }
            else
            {
                var roles = await authService.GetAvailableRoles();
                model.AvailableRoles = roles.Data;
                return BadRequest(result.Message);

            }
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var userPrincipal = HttpContext.User;
            var result = await authService.GetCurrentUserInfoAsync(userPrincipal);
            if (result.IsSuccess)
            {
                var profile = new UserProfileVm 
                {
                    id = result.Data.Id,
                   Name= result.Data.Name                 
                };
                return Ok(profile);
            }

            return Ok(result.Message);
          
        }

        [HttpGet("current-user-role")]
        [Authorize]
        public IActionResult GetCurrentUserRole()
        {
            var user = HttpContext.User;

            // Kiểm tra xem người dùng có tồn tại không
            if (user == null || !user.Identity.IsAuthenticated)
            {
                var result = "User not authenticated";
                return Ok(result);


            }

            // Lấy ra danh sách các roles của người dùng
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);

            if (roles.Any())
            {
                return Ok(roles);
            }
            else
            {
                var result = "Roles not found for the current user";
                return Ok(result);
            }
        }

        [HttpGet("GetAvailableRoles")]
        public async Task<IActionResult> GetAvailableRoles()
        {
            var result = await authService.GetAvailableRoles();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }





    }
}
