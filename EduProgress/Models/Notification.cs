namespace EduProgress.Models;

public enum NotificationCategory { Course, Reminder, System }

public class Notification
{
    public int                  Id        { get; set; }
    public string               Title     { get; set; } = "";
    public string               Message   { get; set; } = "";
    public NotificationCategory Category  { get; set; }
    public DateTime             CreatedAt { get; set; }
    public bool                 IsRead    { get; set; }

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - CreatedAt;
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} мин. назад";
            if (diff.TotalHours   < 24) return $"{(int)diff.TotalHours} ч. назад";
            return $"{(int)diff.TotalDays} д. назад";
        }
    }

    public string CategoryLabel => Category switch
    {
        NotificationCategory.Course   => "Курс",
        NotificationCategory.Reminder => "Напоминание",
        NotificationCategory.System   => "Система",
        _                             => ""
    };
}
