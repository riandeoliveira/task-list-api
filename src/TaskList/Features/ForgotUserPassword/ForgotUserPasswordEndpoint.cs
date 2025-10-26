using Microsoft.AspNetCore.Mvc;
using TaskList.Constants;
using TaskList.Dtos;
using TaskList.Entities;
using TaskList.Interfaces;
using TaskList.ViewModels;

namespace TaskList.Features.ForgotUserPassword;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class ForgotUserEndpoint(
    IAuthService authService,
    II18nService i18n,
    IMailService mailService,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromBody] ForgotUserPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var usernameOrEmail = request.UsernameOrEmail.Trim();

        var user = await userRepository.FindOneAsync(
            x => x.Username == usernameOrEmail || x.Email == usernameOrEmail,
            cancellationToken
        );

        if (user is null)
        {
            return NoContent();
        }

        var token = authService.GenerateJwtToken(user.Id, DateTime.UtcNow.AddMinutes(30));

        var passwordResetToken = new PasswordResetToken
        {
            Value = token.Value,
            ExpiresAt = token.ExpiresAt,
            UserId = user.Id,
        };

        await passwordResetTokenRepository.CreateAsync(passwordResetToken, cancellationToken);

        var resetPasswordUrl =
            $"{EnvironmentVariables.ClientUrl}/reset-password?token={token.Value}";

        var viewModel = new ForgotUserPasswordViewModel(user.Name, resetPasswordUrl);

        var mailSender = new MailSenderDto(
            Recipient: user.Email,
            Subject: i18n.T("PasswordResetRequest"),
            View: new ViewDto("ForgotUserPassword", viewModel)
        );

        await mailService.SendMailAsync(mailSender, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return NoContent();
    }
}
