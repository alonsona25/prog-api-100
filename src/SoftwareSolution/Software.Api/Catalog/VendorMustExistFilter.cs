using Marten;
using Software.Api.Vendors;

namespace Software.Api.Catalog;

public class VendorMustExistFilter(IDocumentSession session, ILogger<VendorMustExistFilter> logger) : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var x = 12;
        if(context.Arguments.Count > 0)
        {
            var vendorId = context.GetArgument<Guid>(0);
            if(vendorId != Guid.Empty)
            {
                var exists = await session.Query<VendorEntity>().Where(v => v.Id == vendorId).AnyAsync();
                if(exists)
                {
                    logger.LogInformation("Before the call to next");
                    return await next(context);
                }

            }
        }
        return TypedResults.NotFound("No Vendor With that Id Bozo");
    }
}
