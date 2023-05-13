using LocationAccess.Database;
using LocationAccess.Models;
using Flurl;
using Flurl.Http;

namespace LocationAccess.Grains; 

public class LocationGrain: Grain, ILocationGrain {
    private readonly LocationContext _context;
    private readonly ILogger<LocationGrain> _logger;
    private readonly long _locationId;
    private Location? _location;
    
    public LocationGrain(LocationContext context, ILogger<LocationGrain> logger) {
        _context = context;
        _logger = logger;
        _locationId = GrainReference.GetPrimaryKeyLong();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken) {
        _location = await _context.Locations.FindAsync(_locationId);
        if (_location == null) {
            _logger.LogInformation("Didn't find Location: {locationId}", _locationId);
        }
        else {
            _logger.LogInformation("Loading Location: {locationId}", _locationId);
        }
        // await base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken) {
        _logger.LogInformation("Unloading Location: {locationId}, exists: {exists}", _locationId, _location != null);
        return Task.CompletedTask;
    }

    public Task<bool> Exists() {
        return Task.FromResult(_location != null);
    }

    public Task<string> GetName() {
        return Task.FromResult(_location?.Name ?? string.Empty);
    }

    public async Task<bool> CallExternal() {
        if (_location == null) {
            return false;
        }

        var host = new Uri(_location.External).Host;
        var external = GrainFactory.GetGrain<IExternalGrain>(host);

        return await external.CallDependency(_locationId, _location.External);
    }
}