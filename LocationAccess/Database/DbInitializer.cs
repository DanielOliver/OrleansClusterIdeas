namespace LocationAccess.Database; 

public static class DbInitializer {
    public static void Initialize(LocationContext context) {
        context.Database.EnsureCreated();
    }
}