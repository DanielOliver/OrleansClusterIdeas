using Microsoft.AspNetCore.Mvc;

namespace ExternalDependencyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ExternalApiController : ControllerBase {

    private readonly ILogger<ExternalApiController> _logger;

    public ExternalApiController(ILogger<ExternalApiController> logger) {
        _logger = logger;
    }

    [HttpGet(Name = "")]
    public async Task<IActionResult> Get() {
        await Task.Delay(Random.Shared.Next(50, 3_000));
        return Ok();
    }
}