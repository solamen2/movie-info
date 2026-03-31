using Microsoft.AspNetCore.Identity;

namespace MovieInfoBackend.Auth;

public class ApplicationUser : IdentityUser
{
    public bool IsSearchUser { get; set; }  // TODO: Refactor to "IsEnabled"
}