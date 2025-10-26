using TaskList.Dtos;

namespace TaskList.Interfaces;

public interface IAuthService
{
    public void ClearAuthCookiesFromClient();

    public AuthTokensDto GenerateAuthTokens(Guid userId);

    public JwtTokenDto GenerateJwtToken(Guid userId, DateTime expiration);

    public Guid GetAuthenticatedUserId();

    public string? GetRefreshTokenFromCookies();

    public void SendAuthCookiesToClient(AuthTokensDto authTokens);
}
