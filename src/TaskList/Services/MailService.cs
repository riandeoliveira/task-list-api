using System.Globalization;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Razor.Templating.Core;
using TaskList.Constants;
using TaskList.Dtos;
using TaskList.Interfaces;

namespace TaskList.Services;

public class MailService : IMailService
{
    public async Task SendMailAsync(MailSenderDto mailSender, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(EnvironmentVariables.MailSender));
        message.To.Add(MailboxAddress.Parse(mailSender.Recipient));
        message.Subject = mailSender.Subject;

        var cultureName = CultureInfo.CurrentCulture.Name;

        var htmlBody = await RazorTemplateEngine.RenderAsync(
            $"/Views/{cultureName}/{mailSender.View.Name}.cshtml",
            mailSender.View.Model
        );

        var body = new BodyBuilder { HtmlBody = htmlBody };

        message.Body = body.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            EnvironmentVariables.MailHost,
            EnvironmentVariables.MailPort,
            SecureSocketOptions.StartTls,
            cancellationToken
        );

        await smtp.AuthenticateAsync(
            EnvironmentVariables.MailUsername,
            EnvironmentVariables.MailPassword,
            cancellationToken
        );

        await smtp.SendAsync(message, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
