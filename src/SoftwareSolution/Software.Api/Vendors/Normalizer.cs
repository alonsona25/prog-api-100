using Slugify;

namespace Software.Api.Vendors;

public class Normalizer : INormalizeVendorNames
{
    //private SlugHelperConfiguration = new SlugHelperConfiguration();
    
public string NormalizeName(string name)
    {
        var config = new SlugHelperConfiguration();
        config.AllowedCharacters.Remove('.');
     
        var helper = new SlugHelper(config);
        var slug = helper.GenerateSlug(name);
        return slug;
    }
}
