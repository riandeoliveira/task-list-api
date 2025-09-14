using TodoList.Dtos;
using TodoList.Requests;

namespace TodoList.Interfaces;

public interface IUserHandler
{
    public Task DeleteAsync(CancellationToken cancellationToken);

    public Task ForgotPasswordAsync(
        ForgotUserPasswordRequest request,
        CancellationToken cancellationToken
    );

    public Task RenewRefreshTokenAsync(CancellationToken cancellationToken);

    public Task ResetPasswordAsync(
        ResetUserPasswordRequest request,
        CancellationToken cancellationToken
    );

    public Task SignInAsync(SignInUserRequest request, CancellationToken cancellationToken);

    public Task SignOutAsync(CancellationToken cancellationToken);

    public Task<UserDto> SignUpAsync(
        SignUpUserRequest request,
        CancellationToken cancellationToken
    );

    public Task<UserDto> UpdateAsync(
        UpdateUserRequest request,
        CancellationToken cancellationToken
    );
}
