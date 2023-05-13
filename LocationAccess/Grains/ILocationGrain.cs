namespace LocationAccess.Grains; 

public interface ILocationGrain: IGrainWithIntegerKey {
    Task<string> GetName();
    Task<bool> Exists();
}