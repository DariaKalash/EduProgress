namespace EduProgress.Models;

public enum CourseStatus { Available, Enrolled, Completed }

public class Module
{
    public int          Id       { get; set; }
    public int          CourseId { get; set; }
    public string       Title    { get; set; } = "";
    public int          Order    { get; set; }
    public List<Lesson> Lessons  { get; set; } = new();
}

public class Course
{
    public int          Id             { get; set; }
    public string       Title          { get; set; } = "";
    public string       Description    { get; set; } = "";
    public string       Category       { get; set; } = "";
    public int          TotalHours     { get; set; }
    public int          EnrolledCount  { get; set; }
    public bool         IsNew          { get; set; }
    public CourseStatus Status         { get; set; } = CourseStatus.Available;
    public int          Progress       { get; set; }   // 0-100
    public List<Module> Modules        { get; set; } = new();

    public int TotalLessons   => Modules.Sum(m => m.Lessons.Count);
    public int DoneLessons    => Modules.Sum(m => m.Lessons.Count(l => l.Status == LessonStatus.Completed));
    public int ModulesCount   => Modules.Count;
}
