using System.Net;
using LocationAccess.Database;
using LocationAccess.Grains;
using LocationAccess.Requests;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Orleans.Configuration;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Sqlite is enough for testing purposes.
builder.Services.AddDbContext<LocationContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IExternalGateway, ExternalGateway>();

// Orleans in-memory and localhost testing
builder.Services.AddOrleans(siloBuilder => {
    // How do I secure clusters?
    // Docker Compose scale workaround.
    siloBuilder.ConfigureEndpoints(Dns.GetHostName(), 10300, 10400, listenOnAnyHostAddress: true);
    
    var useRedis = builder.Configuration.GetValue<bool>("UseRedis");
    logger.Info($"UseRedis: {useRedis} for Orleans", useRedis);
    if(!useRedis) {
        siloBuilder.AddMemoryGrainStorageAsDefault();
        siloBuilder.UseLocalhostClustering();
    } else {
        var redisConnectionString = builder.Configuration.GetConnectionString("RedisDocker");
        siloBuilder.UseRedisClustering(redisConnectionString);
        siloBuilder.AddRedisGrainStorageAsDefault(optionsBuilder => optionsBuilder.Configure(options => {
            options.ConnectionString = redisConnectionString;
            options.DatabaseNumber = 0;
        }));
    }

    siloBuilder.Configure<GrainCollectionOptions>(options => {
        // Messing around with grain limited lifetimes, for the sake of it.
        options.CollectionAge = TimeSpan.FromMinutes(2);
    });
});

builder.Logging.ClearProviders();
builder.Host.UseNLog();


var app = builder.Build();

var createDb = builder.Configuration.GetValue<bool>("CreateDb", true);
if (createDb) {
    using (var scope = app.Services.CreateScope()) {
        DbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<LocationContext>());
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// I'm testing locally without this.
// app.UseHttpsRedirection();
// app.UseAuthorization();
app.MapControllers();
try {
    app.Run();
}
catch (Exception exception) {
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally {
    NLog.LogManager.Shutdown();
}