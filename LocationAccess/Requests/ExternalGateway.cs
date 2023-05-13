using Flurl.Http;
using LocationAccess.Grains;
using Polly;
using Polly.Bulkhead;

namespace LocationAccess.Requests;

/// <summary>
/// Acts as a constraining element for total outbound actions.
/// </summary>
public interface IExternalGateway {
    public Task Request(ExternalRequest request, CancellationToken cancellationToken);
}

public class ExternalGateway: IExternalGateway {
    /// Only two outbound at once.  Up to 1,000 waiting in queue. Not in love with that queue.
    private readonly AsyncBulkheadPolicy _bulkhead = Policy.BulkheadAsync(2, 1_000);
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<ExternalGateway> _logger;
    
    public ExternalGateway(IGrainFactory grainFactory, ILogger<ExternalGateway> logger) {
        _grainFactory = grainFactory;
        _logger = logger;
    }

    public async Task Request(ExternalRequest request, CancellationToken cancellationToken) {
        await _bulkhead.ExecuteAsync((token) => InternalRequest(request, token), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// This could be replaced with any dependency injection or other action. 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    private async Task InternalRequest(ExternalRequest request, CancellationToken cancellationToken) {
        //TODO: verify the partition key is still appropriate. What if configuration changed in the last hour of waiting in queue? 
        //      If partition key has changed, throw it back to beginning of queue?
        try {
            await request.Request.GetAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e) {
            _logger.LogError(e, "Failed request");
        }
        finally {
            // This got me into deadlock once.
            // https://learn.microsoft.com/en-us/dotnet/orleans/grains/reentrancy
            await _grainFactory.GetGrain<IExternalGrain>(request.PartitionKey).NotifyRequestFinished(request).ConfigureAwait(false);
        }
    }
}