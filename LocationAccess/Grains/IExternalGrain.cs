using LocationAccess.Requests;
using Orleans.Concurrency;

namespace LocationAccess.Grains; 

public interface IExternalGrain: IGrainWithStringKey {
    public Task<bool> CallDependency(long locationId, string specific);
    
    /// <summary>
    /// Get out of deadlocking?
    /// </summary>
    [AlwaysInterleave]
    public Task NotifyRequestFinished(ExternalRequest request);
}