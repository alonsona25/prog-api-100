using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Software.Api.Vendors;

[JsonSerializable(typeof(VendorDetailsModel), GenerationMode = JsonSourceGenerationMode.Serialization)]
public record VendorDetailsModel
{
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required VendorPointOfContact PointOfContact { get; init; }
}

public record VendorPointOfContact
{
    public required string Name { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public record VendorCreateModel
{
    [MinLength(5), MaxLength(100)]
    public required string Name { get; init; }
    [Url]
    public required string Url { get; init; }
    public required VendorPointOfContact PointOfContact { get; init; }
}

public record VendorSummaryModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
}

public record VendorSummary
{
    public required IReadOnlyList<VendorSummaryModel> Vendors { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}


public partial record UnusedModel
{

    [GeneratedRegex(@"^(https?://)?([\da-z\.-]+)\.([a-z\.]{2,6})([/\w \.-]*)*/?$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex UrlRegex { get;  }

    public static bool IsValidUrl(string url)
    {
        return UrlRegex.IsMatch(url);
    }
}