public class ProgramConfig
{
    public static readonly TimeSpan LoginCookieTimeout = TimeSpan.FromDays(1);

    // Development-only config
    public static readonly LocalDbConnType DbConnType = LocalDbConnType.Local;
}