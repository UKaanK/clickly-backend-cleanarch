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
        private readonly ILogger<UrlController> _logger;

        public UrlController(IMediator mediator, ILogger<UrlController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("shorten")]
       public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlCommand command)
        {
            _logger.LogInformation("URL kısaltma isteği alındı: Original URL: {OriginalUrl}", command.OriginalUrl);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("stats/{shortCode}")]
        public async Task<IActionResult> GetStats(string shortCode)
        {
            _logger.LogInformation("İstatistik Sorgusu Alındı. Kısa Kod: {ShortCode}", shortCode);
            var response = await _mediator.Send(new GetStatsQuery { ShortCode = shortCode });
            return Ok(response);
        }


    }
}
