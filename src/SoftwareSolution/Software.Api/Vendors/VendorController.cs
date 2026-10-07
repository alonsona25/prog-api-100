using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Software.Api.Vendors;

[ApiController]
[Route("vendors")]
[Authorize]
public class VendorsController : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "SoftwareCenterManager")]
    public async Task<ActionResult<VendorDetailsModel>> CreateVendor([FromBody] VendorCreateModel model, 
        [FromServices] TimeProvider clock, [FromServices] IDocumentSession session, [FromServices] INormalizeVendorNames vendorNameService)
    {
        var normalizedName = vendorNameService.NormalizeName(model.Name);

        // if the name isn't unique, return a 409.
        // 
        var isNameUsed = await session.Query<VendorEntity>()
            .Where(v => v.NormalizedName == normalizedName)
            .AnyAsync();

        //if(isNameUsed)
        //{
        //    return Conflict(new { message = $"A vendor with the name '{model.Name}' already exists." });
        //}

        var vendorEntity = new VendorEntity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Url = model.Url,
            NormalizedName = normalizedName,
            PointOfContact = model.PointOfContact,
            CreatedAt = clock.GetUtcNow(),
            CreatedBy = User.Identity?.Name ?? "Unknown"
        };

        session.Store(vendorEntity);
        await session.SaveChangesAsync();
        // notify the other API of a new vendor.

        var newVendor = new VendorDetailsModel
        {
            Id = vendorEntity.Id,
            Name = vendorEntity.Name,
            Url = vendorEntity.Url,
            PointOfContact = vendorEntity.PointOfContact
        };



        return CreatedAtAction(nameof(GetVendorById), new { id = newVendor.Id }, newVendor);
    }


    [HttpGet("{id:guid}")]

    public async Task<ActionResult<VendorDetailsModel>> GetVendorById(Guid id, [FromServices] IDocumentSession session)
    {

        var response = await session.Query<VendorEntity>()
            .Where(v => v.Id == id)
            .Select(v => new VendorDetailsModel
            {
                Id = v.Id,
                Name = v.Name,
                Url = v.Url,
                PointOfContact = v.PointOfContact
            })
            .SingleOrDefaultAsync(); // if this return nothing, return null, 

        return response switch
        {
            null => NotFound(),
            _ => Ok(response)
        };
    }

    [HttpGet]
    public async Task<ActionResult<VendorSummary>> GetVendors([FromServices] IDocumentSession session, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {

        var vendors = await session.Query<VendorEntity>()
            .OrderBy(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VendorSummaryModel
            {
                Id = v.Id,
                Name = v.Name,
                Url = v.Url
            })
            .ToListAsync();

        var summary = new VendorSummary
        {
            Vendors = vendors,
            Page = page,
            PageSize = pageSize
        };
        return Ok(summary);
    }

}