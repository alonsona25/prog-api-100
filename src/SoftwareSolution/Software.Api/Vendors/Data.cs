namespace Software.Api.Vendors;

// this is our "data model" - what we are saving in the database.
// it will usually exceed your models, and have data attached that is for technical requirements.

public class VendorEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }

    public string NormalizedName { get; set; } = string.Empty;
    public required VendorPointOfContact PointOfContact { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}