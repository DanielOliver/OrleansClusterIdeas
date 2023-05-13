using Microsoft.AspNetCore.Mvc;

namespace LocationAccess.Controllers; 

[ApiController]
[Route("[controller]")]
public class LocationsController : ControllerBase {
    
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILogger<LocationsController> logger) {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public long GetById(long id) {
        return 0;
    }
}