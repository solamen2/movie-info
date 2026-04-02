using Microsoft.AspNetCore.Identity;

namespace MovieInfoBackend.Auth;

public class ApplicationUser : IdentityUser
{
    public bool IsSearchUser { get; set; }  // NOTE: Cannot use any feature of app without this
}