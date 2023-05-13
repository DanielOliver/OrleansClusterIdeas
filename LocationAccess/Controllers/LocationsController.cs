using LocationAccess.Database;
using LocationAccess.Grains;
using LocationAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace LocationAccess.Controllers; 

[ApiController]
[Route("[controller]")]
public class LocationsController : ControllerBase {
    
    private readonly ILogger<LocationsController> _logger;
    private readonly LocationContext _locationContext;
    private readonly IGrainFactory _grainFactory;

    public LocationsController(ILogger<LocationsController> logger, LocationContext locationContext, IGrainFactory grainFactory) {
        _logger = logger;
        _locationContext = locationContext;
        _grainFactory = grainFactory;
    }
    
    [HttpGet("{id}/name")]
    public async Task<IActionResult> GetNameById(long id) {
        var locationGrain = _grainFactory.GetGrain<ILocationGrain>(id);
        var name = await locationGrain.GetName();
        if (string.IsNullOrWhiteSpace(name)) {
            return NotFound();
        }
        return Ok(name);
    }
    [HttpGet("{id}/call")]
    public async Task<IActionResult> CallById(long id) {
        var locationGrain = _grainFactory.GetGrain<ILocationGrain>(id);
        var success = await locationGrain.CallExternal();
        if (!success) {
            return base.BadRequest();
        }
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id) {
        var location = await _locationContext.Locations.FindAsync(id);
        return location == null ? NotFound() : Ok(location);
    }
    
    
    [HttpGet("/call")]
    public async Task<IActionResult> CallAll() {
        foreach (var location in _locationContext.Locations) {
            var locationGrain = _grainFactory.GetGrain<ILocationGrain>(location.ID);
            await locationGrain.CallExternal();
        }
        return Ok();
    }

    [HttpPost("")]
    public async Task<IActionResult> GetById(Location location) {
        var entry = await _locationContext.Locations.AddAsync(location);
        await _locationContext.SaveChangesAsync();
        return Ok(entry.Entity);
    }
}