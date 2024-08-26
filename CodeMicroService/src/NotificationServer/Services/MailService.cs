using MailKit.Net.Smtp;
using MimeKit;
using NotificationServer.Configurations;
using NotificationServer.Templates;
using Shared.Dtos.Emails;

namespace NotificationServer.Services;

public class MailService : IMailService
{
    #region Constructor
    private readonly IConfiguration _configuration;
    public MailService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }
    #endregion

    public async Task SendEmailAsync(EmailBodyDto email)
    {
        var configuration = _configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(configuration.DisplayName, configuration.From));
        emailMessage.To.Add(new MailboxAddress("Rıfkı", email.To));


        if (!string.IsNullOrEmpty(email.Cc))
        {
            emailMessage.Cc.Add(new MailboxAddress("Rıfkı Cc", email.Cc));
            // cc ve bcc için normalde olması gereken array'dir, döngü ile bu alanı çoğaltabilirsiniz.
        }

        if (!string.IsNullOrEmpty(email.Bcc))
        {
            emailMessage.Bcc.Add(new MailboxAddress("Rıfkı Bcc", email.Bcc));
            // cc ve bcc için normalde olması gereken array'dir, döngü ile bu alanı çoğaltabilirsiniz.
        }

        emailMessage.Subject = email.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = email.Body.Info()
        };

        if (email.Attachments.Count > 0)
        {
            foreach (var attachement in email.Attachments)
            {
                bodyBuilder.Attachments.Add(attachement.FileName, attachement.FileContent);
            }
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(configuration.SmtpServer, configuration.Port, false);
                await client.AuthenticateAsync(configuration.Username, configuration.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}