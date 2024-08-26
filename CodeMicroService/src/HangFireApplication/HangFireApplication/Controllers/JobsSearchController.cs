using Hangfire;
using HangFireApplication.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.Jobs;
using System.Net;

namespace HangFireApplication.Controllers;

public class JobsSearchController : Controller
{
    private readonly IBackgroundJobClient _client;
    private readonly IPublishEndpoint _publishEndpoint;
    public JobsSearchController(IBackgroundJobClient backgroundJobClient, IPublishEndpoint publishEndpoint)
    {
        this._client = backgroundJobClient;
        this._publishEndpoint = publishEndpoint;
    }

    // Daha önce yapılmış arama geçmişi yer alabilir :)
    public IActionResult Index() => View();


    public IActionResult Search() => View();

    [HttpPost]
    public IActionResult Search([FromBody] JobSearch model)
    {
        if (ModelState.IsValid)
        {
            if (model.SearchNow)
            {
                foreach (var request in model.Companies)
                {
                    foreach (var keyword in model.KeyWords)
                    {
                        var message = new JobSearchDto
                        {
                            KeyWord = keyword,
                            WebUrl = request
                        };
                        _client.Enqueue(() => SendJob(message));
                    }
                }
            }

            else if (!model.SearchNow && model.ScheduleTime != null && model.ScheduleTime > DateTime.Now)
            {
                foreach (var request in model.Companies)
                {
                    foreach (var keyword in model.KeyWords)
                    {
                        var message = new JobSearchDto
                        {
                            KeyWord = keyword,
                            WebUrl = request
                        };
                        _client.Schedule(() => SendJob(message), model.ScheduleTime.Value);
                    }
                }
            }
        }

        return Json(HttpStatusCode.OK);
    }

    [NonAction]
    public async Task SendJob(JobSearchDto model)
    {
        await _publishEndpoint.Publish(model);
    }
}
