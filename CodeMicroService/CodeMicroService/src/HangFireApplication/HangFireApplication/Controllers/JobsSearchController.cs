using Hangfire;
using HangFireApplication.Models;
using HangFireApplication.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Dtos.Jobs;
using System.Net;

namespace HangFireApplication.Controllers;

public class JobsSearchController : Controller
{
    private readonly IBackgroundJobClient _client;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRedisService<JobSearch> _redisService;
    public JobsSearchController(IBackgroundJobClient backgroundJobClient, IPublishEndpoint publishEndpoint, IRedisService<JobSearch> redisService)
    {
        this._client = backgroundJobClient;
        this._publishEndpoint = publishEndpoint;
        this._redisService = redisService;
    }

    // Daha önce yapılmış arama geçmişi yer alabilir :)
    public IActionResult Index() => View();


    public IActionResult Search()
    {
        var data = _redisService.Get(Consts.RedisConsts.JOBSEARCH_KEY);

        return Json(
                new
                {
                    Status = HttpStatusCode.OK,
                    Data = data,
                    Message = "Tum kayitlar basarili sekilde kaydoldu"
                }
            );
    }

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

        _redisService.Set(Consts.RedisConsts.JOBSEARCH_KEY, model);

        return Json(
                new
                {
                    Status = HttpStatusCode.OK,
                    Data = model,
                    Message = "Islem basarili"
                }
            );
    }

    [NonAction]
    public async Task SendJob(JobSearchDto model)
    {
        await _publishEndpoint.Publish(model);
    }
}
