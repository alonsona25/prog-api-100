using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Software.Api.Vendors;

namespace Software.Api.Catalog.Operations;

public static class Add
{
    public static async Task<Created<CatalogItemDetailsModel>>
        AddCatalogItemAsync(Guid vendorId, IDocumentSession session, CatalogItemCreateModel request)
    {
        
        {
            var entity = new CatalogEntity
            {
                VendorId = vendorId,
                Id = Guid.NewGuid(),
                Name = request.Name
            };
            session.Store(entity);
            await session.SaveChangesAsync();
            var response = new CatalogItemDetailsModel
            {
                Id = entity.Id,
                VendorId = entity.VendorId,
                Name = entity.Name

            };
          

            return TypedResults.Created($"/vendors/{vendorId}/catalog-items/{entity.Id}", response);
        } 
    }
}

public record CatalogItemCreateModel
{
    public required string Name { get; set; }
}

public record CatalogItemDetailsModel
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CatalogEntity
{
    public Guid Id { get; set; }
    public Guid VendorId { get; set; }
    public string Name { get; set; } = string.Empty;
}