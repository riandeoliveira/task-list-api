using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskList.Constants;
using TaskList.Dtos;
using TaskList.Exceptions;
using TaskList.Interfaces;

namespace TaskList.Services;

public class AuthService(IHttpContextAccessor accessor, II18nService i18n) : IAuthService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public void ClearAuthCookiesFromClient()
    {
        var cookies = accessor?.HttpContext?.Response.Cookies;

        cookies?.Delete("access_token");
        cookies?.Delete("refresh_token");
    }

    public AuthTokensDto GenerateAuthTokens(Guid userId)
    {
        var accessToken = GenerateJwtToken(userId, DateTime.UtcNow.AddMinutes(30));
        var refreshToken = GenerateJwtToken(userId, DateTime.UtcNow.AddDays(1));

        return new AuthTokensDto(accessToken, refreshToken);
    }

    public JwtTokenDto GenerateJwtToken(Guid userId, DateTime expiration)
    {
        var claims = new[]
        {
            new Claim("id", userId.ToString()),
            new Claim("uid", Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvironmentVariables.JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = EnvironmentVariables.JwtIssuer,
            Audience = EnvironmentVariables.JwtAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = credentials,
        };

        var securityToken = _tokenHandler.CreateToken(tokenDescriptor);
        var token = _tokenHandler.WriteToken(securityToken);

        return new JwtTokenDto(token, expiration);
    }

    public Guid GetAuthenticatedUserId()
    {
        var authenticatedUserId = accessor?.HttpContext?.User.FindFirst("id")?.Value;

        if (!Guid.TryParse(authenticatedUserId, out var userId))
        {
            throw new UnauthorizedException(i18n.T("UnauthorizedOperation"));
        }

        return userId;
    }

    public string? GetRefreshTokenFromCookies()
    {
        return accessor.HttpContext?.Request.Cookies["refresh_token"];
    }

    public void SendAuthCookiesToClient(AuthTokensDto authTokens)
    {
        var cookies = accessor?.HttpContext?.Response.Cookies;

        cookies?.Append(
            "access_token",
            authTokens.AccessToken.Value,
            GenerateCookie(authTokens.AccessToken.ExpiresAt)
        );

        cookies?.Append(
            "refresh_token",
            authTokens.RefreshToken.Value,
            GenerateCookie(authTokens.RefreshToken.ExpiresAt)
        );
    }

    private static CookieOptions GenerateCookie(DateTime expiresAt)
    {
        return new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt,
        };
    }
}
