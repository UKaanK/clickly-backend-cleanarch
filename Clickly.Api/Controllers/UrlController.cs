using Clickly.Application.Features.GetStats;
using Clickly.Application.Features.ShortenUrl;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clickly.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : Controller
    {
        private readonly IMediator _mediator;

        public UrlController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("shorten")]
       public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        public async Task<IActionResult> GetStats(string shortCode)
        {
            var response = await _mediator.Send(new GetStatsQuery { ShortCode = shortCode });
            return Ok(response);
        }
    }
}
