namespace EduProgress.Models;

public enum UserRole { Administrator = 0, Methodist = 1, Teacher = 2 }

public class User
{
    public int      Id         { get; set; }
    public string   Name       { get; set; } = "";
    public string   Email      { get; set; } = "";
    public string   Login      { get; set; } = "";
    public string   Password   { get; set; } = "";
    public UserRole Role       { get; set; }
    public DateTime LastActive { get; set; }
    public bool     IsActive   { get; set; } = true;

    public string Initials
    {
        get
        {
            var p = Name.Split(' ');
            if (p.Length >= 2) return $"{p[0][0]}{p[1][0]}".ToUpper();
            return Name.Length > 0 ? Name[0].ToString().ToUpper() : "?";
        }
    }

    public string RoleLabel => Role switch
    {
        UserRole.Administrator => "Администратор",
        UserRole.Methodist     => "Методист",
        UserRole.Teacher       => "Преподаватель",
        _                      => ""
    };
}
