using Microsoft.AspNetCore.Identity;

namespace datingApp.Entities;

public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}