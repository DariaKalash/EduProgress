namespace EduProgress.Models;

public class FaqItem
{
    public int    Id          { get; set; }
    public string Category    { get; set; } = "";   // course | system | technical
    public string Question    { get; set; } = "";
    public string Answer      { get; set; } = "";
    public int    OrderNumber { get; set; }
}
