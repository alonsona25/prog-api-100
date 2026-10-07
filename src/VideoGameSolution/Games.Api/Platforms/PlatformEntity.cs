namespace Games.Api.Platforms;

public class PlatformEntity
{
    public required Guid Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required DateTimeOffset Created { get; set; }
}
