using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using MovieInfoBackend.Auth;
using MovieInfoBackend.Helpers;

namespace TestMovieInfoBackend.Auth;

public class AdditionalUserClaimsPrincipalFactoryTests
{
    private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> roleManagerMock;
    private readonly Mock<IOptions<IdentityOptions>> optionsAccessorMock;
    private readonly AdditionalUserClaimsPrincipalFactory factory;

    public AdditionalUserClaimsPrincipalFactoryTests()
    {
        #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

        roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            Mock.Of<IRoleStore<IdentityRole>>(),
            null, null, null, null);
        #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        var identityOptions = new IdentityOptions();
        optionsAccessorMock = new Mock<IOptions<IdentityOptions>>();
        optionsAccessorMock.Setup(x => x.Value).Returns(identityOptions);

        factory = new AdditionalUserClaimsPrincipalFactory(
            userManagerMock.Object,
            roleManagerMock.Object,
            optionsAccessorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_UserWithEmailAndIsSearchUser_ReturnsClaimsPrincipalWithBothClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-id",
            UserName = "testuser",
            Email = "test@example.com",
            IsSearchUser = true
        };

        userManagerMock.Setup(x => x.GetUserIdAsync(user))
            .ReturnsAsync(user.Id);
        userManagerMock.Setup(x => x.GetUserNameAsync(user))
            .ReturnsAsync(user.UserName);
        userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        // Act
        ClaimsPrincipal result = await factory.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Identity);

        ClaimsIdentity? identity = result.Identity as ClaimsIdentity;
        Assert.NotNull(identity);
        Assert.True(result.Identity.IsAuthenticated);

        var claims = identity.Claims.ToList();

        // Should have EmailAddressClaim for logged-in users
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == ProgramConstants.LoggedInUsersOnlyPolicyClaimName);

        // Should have IsSearchUserClaim for search users
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == ProgramConstants.SearchUsersOnlyPolicyClaimName);
    }

    [Fact]
    public async Task CreateAsync_UserWithoutEmailAndNotSearchUser_ReturnsClaimsPrincipalWithNoClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-id",
            UserName = "testuser",
            Email = null,
            IsSearchUser = false
        };

        userManagerMock.Setup(x => x.GetUserIdAsync(user))
            .ReturnsAsync(user.Id);
        userManagerMock.Setup(x => x.GetUserNameAsync(user))
            .ReturnsAsync(user.UserName);
        userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        // Act
        ClaimsPrincipal result = await factory.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Identity);

        ClaimsIdentity? identity = result.Identity as ClaimsIdentity;
        Assert.NotNull(identity);
        Assert.True(result.Identity.IsAuthenticated);

        var claims = identity.Claims.ToList();

        // Should NOT have EmailAddressClaim (no email)
        Assert.DoesNotContain(claims, c => c.Type == ClaimTypes.Role && c.Value == ProgramConstants.LoggedInUsersOnlyPolicyClaimName);

        // Should NOT have IsSearchUserClaim
        Assert.DoesNotContain(claims, c => c.Type == ClaimTypes.Role && c.Value == ProgramConstants.SearchUsersOnlyPolicyClaimName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public async Task CreateAsync_UserWithWhitespaceEmail_DoesNotAddEmailClaim(string whitespaceEmail)
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-id",
            UserName = "testuser",
            Email = whitespaceEmail,
            IsSearchUser = true
        };

        userManagerMock.Setup(x => x.GetUserIdAsync(user))
            .ReturnsAsync(user.Id);
        userManagerMock.Setup(x => x.GetUserNameAsync(user))
            .ReturnsAsync(user.UserName);
        userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        // Act
        ClaimsPrincipal result = await factory.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Identity);
        Assert.True(result.Identity.IsAuthenticated);

        ClaimsIdentity? identity = result.Identity as ClaimsIdentity;
        Assert.NotNull(identity);

        var claims = identity.Claims.ToList();

        // Should NOT have EmailAddressClaim (whitespace email)
        Assert.DoesNotContain(claims, c => c.Type == ClaimTypes.Role && c.Value == ProgramConstants.LoggedInUsersOnlyPolicyClaimName);

        // Should still have IsSearchUserClaim
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == ProgramConstants.SearchUsersOnlyPolicyClaimName);
    }

    [Fact]
    public async Task CreateAsync_ValidUser_HasBaseClaimsFromIdentity()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-id-123",
            UserName = "testuser123",
            Email = "test123@example.com",
            IsSearchUser = false
        };

        userManagerMock.Setup(x => x.GetUserIdAsync(user))
            .ReturnsAsync(user.Id);
        userManagerMock.Setup(x => x.GetUserNameAsync(user))
            .ReturnsAsync(user.UserName);
        userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        // Act
        ClaimsPrincipal result = await factory.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Identity);

        ClaimsIdentity? identity = result.Identity as ClaimsIdentity;
        Assert.NotNull(identity);
        Assert.True(result.Identity.IsAuthenticated);

        var claims = identity.Claims.ToList();

        // Should have base claims from UserClaimsPrincipalFactory (user ID and username)
        Assert.Contains(claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id);
        Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
    }
}