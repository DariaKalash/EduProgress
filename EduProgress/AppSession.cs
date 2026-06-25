using EduProgress.Models;
using EduProgress.Services;

namespace EduProgress;

public static class AppSession
{
    public static User?           CurrentUser  { get; set; }
    public static DatabaseService Db           { get; } = new DatabaseService();
    public static Action<string, object?>? NavigateTo { get; set; }

    public static string GetRoleLabel() => CurrentUser?.Role switch
    {
        UserRole.Administrator => "Администратор",
        UserRole.Methodist     => "Методист",
        UserRole.Teacher       => "Преподаватель",
        _                      => ""
    };
}
