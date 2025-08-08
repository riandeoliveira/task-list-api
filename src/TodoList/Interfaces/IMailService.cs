using TodoList.Dtos;

namespace TodoList.Interfaces;

public interface IMailService
{
    public Task SendMailAsync(MailSenderDto mailSender, CancellationToken cancellationToken);
}
