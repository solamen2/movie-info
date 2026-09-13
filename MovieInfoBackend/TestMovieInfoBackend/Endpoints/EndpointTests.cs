using Xunit.Abstractions;

namespace TestMovieInfoBackend.Endpoints;

public class EndpointTests
{
    public EndpointTests(ITestOutputHelper output)
    {

    }

    [Fact]
    public void Endpoint_EndpointAttributes_Exist()
    {
        // Arrange & Act & Assert

        // Verify authorization policy names exist
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.LoggedInUsersOnlyPolicyName);
        Assert.NotNull(MovieInfoBackend.Helpers.ProgramConstants.SearchUsersOnlyPolicyName);
    }
}

