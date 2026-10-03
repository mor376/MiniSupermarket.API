namespace MiniSupermarket.WinForms
{
    public static class SessionManager
    {
        public static string JwtToken { get; set; } = string.Empty;
        public static string CurrentUsername { get; set; } = string.Empty;
        public static string CurrentRole { get; set; } = string.Empty;

        public static bool IsLoggedIn => !string.IsNullOrEmpty(JwtToken);
    }
}