using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace TaskList.Dtos;

public record ProblemDetailsDto(
    string Detail,
    int Status,
    string Instance,
    Dictionary<string, string[]>? Errors = null
)
{
    [JsonPropertyName("type")]
    public string Type => $"https://httpstatuses.com/{Status}";

    [JsonPropertyName("title")]
    public string Title => ReasonPhrases.GetReasonPhrase(Status);

    [JsonPropertyName("status")]
    public int Status { get; init; } = Status;

    [JsonPropertyName("detail")]
    public string Detail { get; init; } = Detail;

    [JsonPropertyName("instance")]
    public string Instance { get; init; } = Instance;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; init; } = Errors;
}
