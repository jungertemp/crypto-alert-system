using CryptoAlert.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace CryptoAlert.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricesController : ControllerBase
{
    private readonly IPriceHistoryQueryService _priceHistoryQueryService;

    public PricesController(IPriceHistoryQueryService priceHistoryQueryService)
    {
        _priceHistoryQueryService = priceHistoryQueryService;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        [FromQuery] string assetId,
        [FromQuery] int hours = 24,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(assetId))
            return BadRequest("assetId is required.");

        var result = await _priceHistoryQueryService.GetHistoryAsync(
            assetId,
            hours,
            cancellationToken);

        return Ok(result);
    }
}