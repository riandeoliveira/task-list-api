namespace TaskList.Dtos;

public record AuthTokensDto(JwtTokenDto AccessToken, JwtTokenDto RefreshToken);
