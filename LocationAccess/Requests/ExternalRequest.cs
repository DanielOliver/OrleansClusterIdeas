namespace LocationAccess.Requests; 

[GenerateSerializer]
public class ExternalRequest {
    public Guid Id { get; set; } = Guid.NewGuid();
    public long LocationId { get; set; }
    public Uri Request { get; set; }
    public string PartitionKey { get; set; }
    public DateTime RequestedUtc { get; set; } = DateTime.UtcNow;
}