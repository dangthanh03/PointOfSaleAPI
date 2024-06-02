using Microsoft.AspNetCore.Identity;

namespace DoAnAPI.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
