namespace TodoList.Dtos;

public record AuthTokensDto(JwtTokenDto AccessToken, JwtTokenDto RefreshToken);
