namespace EduProgress.Models;

public enum LessonStatus { NotStarted, InProgress, Completed }

public class LessonResource
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "pdf"; // pdf, video, link
    public string Url  { get; set; } = "";
}

public class Lesson
{
    public int                  Id              { get; set; }
    public int                  ModuleId        { get; set; }
    public string               Title           { get; set; } = "";
    public string               Content         { get; set; } = "";
    public int                  DurationMinutes { get; set; }
    public string?              ZoomLink        { get; set; }
    public List<LessonResource> Resources       { get; set; } = new();
    public LessonStatus         Status          { get; set; } = LessonStatus.NotStarted;
    public int                  Order           { get; set; }

    public string DurationLabel =>
        DurationMinutes >= 60
            ? $"{DurationMinutes / 60} ч {DurationMinutes % 60} мин"
            : $"{DurationMinutes} мин";
}
