namespace EduProgress.Services;

/// <summary>
/// Строка подключения к MS SQL Server.
/// Измените значение ConnectionString перед запуском приложения.
/// </summary>
public static class DbConfig
{
    // ── Вариант 1: Windows-аутентификация (рекомендуется для локальной разработки) ──
    public static string DefaultCs = "Server=DESKTOP-D08RGSP\\SQLEXPRESS;Database=EduProgress;Trusted_Connection=True;TrustServerCertificate=True;";

    // ── Вариант 2: SQL Server-аутентификация ──
    // private const string DefaultCs =
    //     "Server=localhost;Database=EduProgress;User Id=sa;Password=YourPassword;TrustServerCertificate=True;";

    private static string _connectionString = DefaultCs;

    public static string ConnectionString
    {
        get => _connectionString;
        set => _connectionString = value;
    }

    /// <summary>Проверяет доступность БД.</summary>
    public static bool TestConnection(out string errorMessage)
    {
        errorMessage = string.Empty;
        try
        {
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            conn.Open();
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
}
