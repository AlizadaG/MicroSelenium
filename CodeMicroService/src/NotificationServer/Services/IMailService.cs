using Shared.Dtos.Emails;

namespace NotificationServer.Services;

public interface IMailService
{
    Task SendEmailAsync(EmailBodyDto email);
}
