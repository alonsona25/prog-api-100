using System.ComponentModel.DataAnnotations;

namespace Games.Api.Platforms;

public record PlatformCreateRequest
{
    [Required, MinLength(3), MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

public record PlatformDetailsResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required DateTimeOffset Created { get; set; }
}