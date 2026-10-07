using Marten;
using Software.Api.Catalog.Operations;

namespace Software.Api.Catalog;

public static class Endpoints
{

    extension(IServiceCollection services)
    {
        public IServiceCollection AddCatalogServices()
        {
            //services.AddScoped<INormalizeVendorNames, Normalizer>();
            return services;
        }
    }
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder MapCatalog()
        {
            var catalogGroup = routeBuilder.MapGroup("/vendors/{vendorId:guid}/catalog")
               .WithTags("catalog", "vendors")
                .WithDescription("YEah, this is the description")
                .WithDisplayName("Cataloggin'");

            catalogGroup.MapPost("", Add.AddCatalogItemAsync)
                .AddEndpointFilter(async (context,next) =>
                {
                    return await next(context);
                })
                .AddEndpointFilter<VendorMustExistFilter>();
            var individualVendorGroup = catalogGroup.MapGroup("{catalogItemId:guid}");
            individualVendorGroup.MapGet("", Get.GetByIdAsync);
            catalogGroup.MapGet("", Get.GetAllAsync);
            return routeBuilder;
        }
    }
}
