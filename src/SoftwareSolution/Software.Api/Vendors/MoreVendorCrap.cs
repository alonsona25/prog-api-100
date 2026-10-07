using Microsoft.AspNetCore.Mvc;


namespace Software.Api.Vendors;

public class MoreVendorCrap : ControllerBase
{
    [HttpPost("/vendors/{id:guid}/catalog")]
    public async Task<ActionResult> DoIt(Guid id)
    {
        return Ok();
    }
}
