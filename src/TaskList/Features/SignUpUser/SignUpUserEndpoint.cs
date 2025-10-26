using Microsoft.AspNetCore.Mvc;
using TaskList.Constants;
using TaskList.Dtos;
using TaskList.Entities;
using TaskList.Exceptions;
using TaskList.Interfaces;
using TaskList.ViewModels;

namespace TaskList.Features.SignUpUser;

[ApiController]
[Produces("application/json", "application/problem+json")]
[Route("api/users")]
[Tags("User")]
public class SignUpUserEndpoint(
    IAuthService authService,
    II18nService i18n,
    IPasswordService passwordService,
    IPersonalRefreshTokenRepository personalRefreshTokenRepository,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository
) : ControllerBase
{
    [HttpPost("sign-up")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(ProblemDetailsDto))]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError,
        Type = typeof(ProblemDetailsDto)
    )]
    public async Task<IActionResult> ExecuteAsync(
        [FromBody] SignUpUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var name = request.Name.Trim();
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var password = request.Password.Trim();

        var userAlreadyExists = await userRepository.ExistAsync(
            x => x.Username == username || x.Email == email,
            cancellationToken
        );

        if (userAlreadyExists)
        {
            throw new ConflictException(i18n.T("UserAlreadyExists"));
        }

        var hashedPassword = passwordService.HashPassword(password);

        var user = new User
        {
            Name = name,
            Username = username,
            Email = email,
            Password = hashedPassword,
        };

        await userRepository.CreateAsync(user, cancellationToken);

        var authTokens = authService.GenerateAuthTokens(user.Id);

        var personalRefreshToken = new PersonalRefreshToken()
        {
            Value = authTokens.RefreshToken.Value,
            ExpiresAt = authTokens.RefreshToken.ExpiresAt,
            UserId = user.Id,
        };

        await personalRefreshTokenRepository.CreateAsync(personalRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        authService.SendAuthCookiesToClient(authTokens);

        var response = new UserDto(user.Id, user.Name, user.Username, user.Email, user.CreatedAt);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}
