using Microsoft.AspNetCore.Mvc.Testing;
using Shard.RayanCedric.API;
using Shard.Shared.Web.IntegrationTests;
using Xunit.Abstractions;

namespace Shard.RayanCedric.IntegrationTests;

public class IntegrationTests : BaseIntegrationTests<Program>
{
    public IntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
        : base(factory, testOutputHelper)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    }
}