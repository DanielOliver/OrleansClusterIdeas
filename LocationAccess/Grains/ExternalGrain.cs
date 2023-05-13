using System.Net;
using Flurl.Http;
using LocationAccess.Requests;
using Orleans.Concurrency;

namespace LocationAccess.Grains; 

public class ExternalGrain: Grain, IExternalGrain {
    
    private readonly ILogger<ExternalGrain> _logger;
    private readonly string _key;
    private readonly IExternalGateway _externalGateway;
    
    public ExternalGrain(ILogger<ExternalGrain> logger, IExternalGateway externalGateway) {
        _logger = logger;
        _externalGateway = externalGateway;
        _key = GrainReference.GetPrimaryKeyString();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Loading External: {key}", _key);
        await Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken) {
        _logger.LogInformation("Unloading External: {key}", _key);
        return Task.CompletedTask;
    }
    
    public async Task<bool> CallDependency(long locationId, string specific) {
        // try {
        //     var result = await specific.GetAsync();
        //     return result.StatusCode == (int)HttpStatusCode.OK;
        // }
        // finally {
        //     _logger.LogInformation("Called External: {key}, location {id}, specific {specific}", _key, locationId, specific);
        // }
        var request = new ExternalRequest() {
            Id = Guid.NewGuid(),
            LocationId = locationId,
            PartitionKey = _key,
            Request = new Uri(specific),
            RequestedUtc = DateTime.UtcNow
        };
        try {
            await _externalGateway.Request(request, default);
        }
        finally {
            _logger.LogInformation("Called External: {key}, location: {id}, request \"{request}\", Guid: {guid}", _key, locationId, request.Request, request.Id);
        }
        return true;
    }

    public Task NotifyRequestFinished(ExternalRequest request) {
        _logger.LogInformation("Request: {requestId} is finished", request.Id);
        return Task.CompletedTask;
    }
}