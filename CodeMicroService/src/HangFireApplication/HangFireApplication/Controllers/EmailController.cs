using Hangfire;
using HangFireApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Refit;


namespace HangFireApplication.Controllers;

public class EmailController : Controller
{
    private readonly IBackgroundJobClient _client;
    public EmailController(IBackgroundJobClient backgroundJobClient)
    {
        this._client = backgroundJobClient;
    }

    public IActionResult Send() => View();

    [HttpPost]
    public IActionResult Send(EmailBodyDto model, IFormFile[] attachments)
    {

        if (attachments != null && attachments.Length > 0)
        {
            model.Attachments = [];
            foreach (var file in attachments)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    model.Attachments.Add(new AttachmentDto
                    {
                        FileName = file.FileName,
                        FileContent = fileBytes
                    });
                }
            }
        }

        if (model.SendNow) // mail anlık olarak gönderilecekse,
        {
            _client.Enqueue(() => SendEmailJobAsync(model));
        }

        if (!model.SendNow && model.ScheduleTime != null)
        {
            var timeUntilSendDate = model.ScheduleTime.Value - DateTime.Now;
            _client.Schedule(() => SendEmailJobAsync(model), timeUntilSendDate);
        }

        return View(model);
    }


    [NonAction]
    public async Task SendEmailJobAsync(EmailBodyDto request)
    {
        var mailServiceApi = RestService.For<IMailServiceApi>("http://localhost:5020");
        await mailServiceApi.SendEmailAsync(request);
    }
}
