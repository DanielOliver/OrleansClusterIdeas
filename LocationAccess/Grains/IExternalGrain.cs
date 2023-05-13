namespace LocationAccess.Grains; 

public interface IExternalGrain: IGrainWithStringKey {
    public Task<bool> CallDependency(long locationId, string specific);
}