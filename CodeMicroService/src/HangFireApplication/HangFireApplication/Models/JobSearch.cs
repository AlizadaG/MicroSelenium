namespace HangFireApplication.Models;

public class JobSearch
{
    public string[]? KeyWords { get; set; }
    public string[]? Companies { get; set; }
    public bool SearchNow { get; set; }
    public DateTime? ScheduleTime { get; set; }
}
