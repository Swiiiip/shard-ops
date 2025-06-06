using Prometheus;
using System.Reflection;
using DotNetEnv;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Shard.API.BackgroundServices;
using Shard.API.Configuration;
using Shard.API.Gateway;
using Shard.API.Model.Buildings;
using Shard.API.Model.Buildings.ConstructionBuildings;
using Shard.API.Model.Buildings.EconomicBuildings;
using Shard.API.Model.Units;
using Shard.API.Model.Units.BasicUnits;
using Shard.API.Model.Units.CombatUnits;
using Shard.API.Repositories.Universe;
using Shard.API.Repositories.Users;
using Shard.API.Services;
using Shard.Shared.Core;
using CombatService = Shard.API.Services.CombatService;
using SystemClock = Shard.Shared.Core.SystemClock;
using UserService = Shard.API.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

var isProd = builder.Environment.IsProduction();
string version;

if (isProd)
{
    version = VersionHandler.GetVersion();
}else
{
    version = "Development";
    
    var envPath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.FullName, ".env");
    Env.Load(envPath);
}

if (args is ["--version"])
{
    Console.WriteLine(version);
    return;
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(version, new OpenApiInfo
    {
        Version = version,
        Title = "Shard.API",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
    
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<ISectorRepository, SectorRepository>();

if (isProd)
{
    BsonClassMap.RegisterClassMap<Unit>(cm =>
    {
        cm.AutoMap();
        cm.SetIsRootClass(true);
        cm.SetDiscriminatorIsRequired(true);
        cm.AddKnownType(typeof(Scout));
        cm.AddKnownType(typeof(Builder));
        cm.AddKnownType(typeof(CombatUnit));
    });

    BsonClassMap.RegisterClassMap<CombatUnit>(cm =>
    {
        cm.AutoMap();
        cm.SetDiscriminatorIsRequired(true);
        cm.AddKnownType(typeof(Bomber));
        cm.AddKnownType(typeof(Fighter));
        cm.AddKnownType(typeof(Cruiser));
    });

    BsonClassMap.RegisterClassMap<Building>(cm =>
    {
        cm.AutoMap();
        cm.SetIsRootClass(true);
        cm.SetDiscriminatorIsRequired(true);
        cm.AddKnownType(typeof(Mine));
        cm.AddKnownType(typeof(StarPort));
    });
    builder.Services.AddSingleton<IMongoClient, MongoClient>(
        _ => new MongoClient(builder.Configuration["MongoDB:BaseUri"])
    );
    builder.Services.AddSingleton<IUserRepository, MongoDBUserRepository>();
}

else
{
    builder.Services.AddSingleton<IUserRepository, LocalMemoryUserRepository>();
    builder.Services.Configure<AdminCredentials>(builder.Configuration.GetSection("AdminCredentials"));
    builder.Services.Configure<Wormholes>(builder.Configuration.GetSection("Wormholes"));
}

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IShardGateway, ShardGatewayHttpClientFactory>();

builder.Services.AddSingleton<SectorService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<UnitService>();
builder.Services.AddSingleton<BuildingService>();
builder.Services.AddSingleton<CombatService>();

builder.Services.AddHostedService<CombatBackgroundService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Shard.API {version}");
});


app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();

app.UseHttpMetrics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.Run();

namespace Shard.API
{
    public class Program
    {

    }
}