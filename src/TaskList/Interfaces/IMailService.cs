using TaskList.Dtos;

namespace TaskList.Interfaces;

public interface IMailService
{
    public Task SendMailAsync(MailSenderDto mailSender, CancellationToken cancellationToken);
}
