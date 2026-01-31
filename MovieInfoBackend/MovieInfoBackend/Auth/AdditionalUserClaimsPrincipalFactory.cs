using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MovieInfoBackend.Auth;
using MovieInfoBackend.Helpers;

public class AdditionalUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public AdditionalUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    { }

    public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity;

        var claims = new List<Claim>();

        if (!String.IsNullOrWhiteSpace(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Role, ProgramConstants.LoggedInUsersOnlyPolicyClaimName));
        }
        if (user.IsSearchUser)
        {
            claims.Add(new Claim(ClaimTypes.Role, ProgramConstants.SearchUsersOnlyPolicyClaimName));
        }

        identity.AddClaims(claims);
        return principal;
    }
}