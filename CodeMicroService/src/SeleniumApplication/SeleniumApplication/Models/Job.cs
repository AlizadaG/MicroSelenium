namespace SeleniumApplication.Models;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Keyword { get; set; }

    //[Obsolete("This property is obsolete. Use CompanyName instead.")]
    public string CompanyName { get; set; }
}
