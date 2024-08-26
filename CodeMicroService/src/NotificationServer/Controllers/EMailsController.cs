using Microsoft.AspNetCore.Mvc;
using NotificationServer.Services;
using Shared.Dtos.Emails;

namespace NotificationServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EMailsController : ControllerBase
{
    private readonly IMailService _mailService;
    public EMailsController(IMailService mailService)
    {
        this._mailService = mailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMail([FromBody] EmailBodyDto request)
    {
        try
        {
            await _mailService.SendEmailAsync(request);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
