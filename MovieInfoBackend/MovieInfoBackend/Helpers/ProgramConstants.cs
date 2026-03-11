namespace MovieInfoBackend.Helpers;

public static class ProgramConstants
{
        public static string LoggedInUsersOnlyPolicyName => "LoggedInUsersOnlyPolicy";
        public static string LoggedInUsersOnlyPolicyClaimName => "EmailAddressClaim";
        public static string SearchUsersOnlyPolicyName => "SearchUsersOnlyPolicy";
        public static string SearchUsersOnlyPolicyClaimName => "IsSearchUserClaim";
        public static string ApiRoutePrefix = "/api";
}