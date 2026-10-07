using Marten;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Software.Api.Catalog.Operations;

public static class Get
{

    public static async Task<Ok<IReadOnlyList<CatalogItemDetailsModel>>> GetAllAsync(
        IDocumentSession session,
        CancellationToken token) => TypedResults.Ok(await session.Query<CatalogEntity>().Select(c => new CatalogItemDetailsModel { Id = c.Id, VendorId = c.VendorId, Name = c.Name }).ToListAsync(token));
    
    public static async Task<Results<Ok<CatalogItemDetailsModel>, NotFound>> GetByIdAsync(Guid vendorId, Guid catalogItemId, IDocumentSession session)
    {
        var response = await session.Query<CatalogEntity>()
            .Where(c => c.Id == catalogItemId && c.VendorId == vendorId)
            .Select(c => new CatalogItemDetailsModel
            {
                Id = c.Id,
                VendorId = c.VendorId,
                Name = c.Name
            })
            .SingleOrDefaultAsync();
        return response switch
        {
            null => TypedResults.NotFound(),
            _ => TypedResults.Ok(response)
        };
    }

}
