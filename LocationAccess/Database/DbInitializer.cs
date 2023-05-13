using LocationAccess.Models;

namespace LocationAccess.Database; 

public static class DbInitializer {
    private static Dictionary<string, int> databaseRows = new Dictionary<string, int>() {
        { "http://external1:80/externalapi", 10 },
        { "http://external2:80/externalapi", 5 },
        { "http://external3:80/externalapi", 3 },
        { "http://external1:80/externalapi/200", 3 },
        { "http://external2:80/externalapi/200", 3 },
        { "http://external2:80/externalapi/2000", 3 },
        { "http://external3:80/externalapi/200", 3 }
    };

    public static void Initialize(LocationContext context) {
        context.Database.EnsureCreated();

        foreach (var row in databaseRows) {
            context.Locations.AddRange(
                Enumerable.Range(1, row.Value).Select(index => new Location
                {
                    External = row.Key,
                    Name = $"Row {index}"
                })
                );
        }
        context.SaveChanges();
    }
}