using Software.Api.Vendors;

namespace Software.Tests;

[Trait("Level", "UnitTest")]
public class NormalizerTests
{
    [Theory]
    [InlineData("Test Vendor", "test-vendor")]
    [InlineData("Test Vendor Inc.", "test-vendor-inc")]
    [InlineData("  bob's house of pancakes   ", "bobs-house-of-pancakes")]
    [InlineData("Bill & Ted's Excellent Adventure", "bill-teds-excellent-adventure")]
    public void CanNormalizeVendorName(string input, string expected)
    {
        var normalizer = new Normalizer();
        var result = normalizer.NormalizeName(input);
        Assert.Equal(expected, result);
    }
}
