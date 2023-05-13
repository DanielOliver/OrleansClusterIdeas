using System.Net;
using Flurl.Http;

namespace LocationAccess.Grains; 

public class ExternalGrain: Grain, IExternalGrain {
    
    private readonly ILogger<ExternalGrain> _logger;
    private readonly string _key;
    
    public ExternalGrain(ILogger<ExternalGrain> logger) {
        _logger = logger;
        _key = GrainReference.GetPrimaryKeyString();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Loading External: {key}", _key);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken) {
        _logger.LogInformation("Unloading External: {key}", _key);
        return Task.CompletedTask;
    }
    
    public async Task<bool> CallDependency(long locationId, string specific) {
        try {
            var result = await specific.GetAsync();
            return result.StatusCode == (int)HttpStatusCode.OK;
        }
        finally {
            _logger.LogInformation("Called External: {key}, location {id}, specific {specific}", _key, locationId, specific);
        }
    }
}