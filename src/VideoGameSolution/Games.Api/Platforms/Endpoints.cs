using Marten;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Games.Api.Platforms;

[ApiController]
public class Endpoints(IDocumentSession session, TimeProvider clock) : ControllerBase
{
    //private IDocumentSession session;

    //public Endpoints(IDocumentSession session)
    //{
    //    this.session = session;
    //}

    // POST /platforms
    [HttpPost("/platforms")]
    public async Task<ActionResult> AddPlatformAsync([FromBody] PlatformCreateRequest request)
    {

        // If we got here, it is "valid"
        // take the request and insert it into the database. (create an entity)
        // save the changes
        // turn it into the model response.
        var entity = new PlatformEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Created = clock.GetUtcNow()
        };
        session.Store(entity);
        await session.SaveChangesAsync();

        var response = new PlatformDetailsResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Created = entity.Created
        };
        
        return Ok(response);
    }
    [HttpGet("/platforms")]
    public async Task<ActionResult<IReadOnlyList<PlatformDetailsResponse>>> GetAllPlatforms()
    {
        var response = await session.Query<PlatformEntity>()
            .Select(p => new PlatformDetailsResponse { Id = p.Id, Name = p.Name, Created = p.Created })
            .ToListAsync();

        return Ok(response);
        
    }
}

