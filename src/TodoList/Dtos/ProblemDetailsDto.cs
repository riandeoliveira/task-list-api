using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace TodoList.Dtos;

public record ProblemDetailsDto(string Detail, int Status, string Instance, object? Errors = null)
{
    public string Type => $"https://httpstatuses.com/{Status}";

    public string Title => ReasonPhrases.GetReasonPhrase(Status);

    public int Status { get; init; } = Status;

    public string Detail { get; init; } = Detail;

    public string Instance { get; init; } = Instance;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Errors { get; init; } = Errors;
}
