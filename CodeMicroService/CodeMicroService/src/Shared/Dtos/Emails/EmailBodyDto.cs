namespace Shared.Dtos.Emails;

public class EmailBodyDto
{
    public string To { get; set; } = null!;
    public string From { get; set; } = null!;
    public string? Bcc { get; set; }  // gizli tanık :)
    public string? Cc { get; set; } // gizsiz tanık :)
    public string Subject { get; set; } = null!;
    public string? Body { get; set; }
    public bool SendNow { get; set; } = true;
    public DateTime? ScheduleTime { get; set; }
    public ICollection<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
}
