using Microsoft.AspNetCore.Mvc;

namespace ExternalDependencyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ExternalApiController : ControllerBase {

    private readonly ILogger<ExternalApiController> _logger;

    public ExternalApiController(ILogger<ExternalApiController> logger) {
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get() {
        await Task.Delay(Random.Shared.Next(50, 3_000));
        return Ok();
    }
    
    [HttpGet("{maxDelay:int}")]
    public async Task<IActionResult> Get(int maxDelay) {
        await Task.Delay(Random.Shared.Next(10, Math.Max(60,Math.Min(15_000, maxDelay))));
        return Ok();
    }
}