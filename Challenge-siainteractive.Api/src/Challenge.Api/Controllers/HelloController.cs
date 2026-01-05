using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Challenge.Api.Controllers;

[ApiController]
[ApiVersion("1.0")] 
[Route("api/v{version:apiVersion}/[controller]")]
public class HelloController : Controller
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public HelloController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Produces("application/json")]
    public async Task<IActionResult> GetHello()
    {
        var text = "hello!";

        return Ok(text);
    }

}
