using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Shard.API.Configuration;
using Shard.API.Model.Units;
using Shard.API.Model.Users;

namespace Shard.API.Gateway;
public class ShardGatewayHttpClientFactory : IShardGateway
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly Wormholes _wormholes;

    private const string AuthenticationScheme = "Basic";

    public ShardGatewayHttpClientFactory(IHttpClientFactory clientFactory, IOptions<Wormholes> wormholes)
    {
        _clientFactory = clientFactory;
        _wormholes = wormholes.Value;
    }

    public async Task<string> ExecuteJump(string serverName, User user, Unit unit)
    {
        var server = _wormholes[serverName];
        var client = CreateHttpClient(serverName, server);

        var baseUri = server.BaseUri;
        
        await UpdateUser(client, baseUri, user);
        await UpdateUnit(client, baseUri, user.Id, unit);

        return BuildFullRequestPath(baseUri, $"/users/{user.Id}/units/{unit.Id}");
    }

    private HttpClient CreateHttpClient(string serverName, Server server)
    {
        var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = CreateAuthorizationHeader(serverName, server.SharedPassword);
        return client;
    }

    private async Task UpdateUser(HttpClient client, string baseUri, User user)
    {
        var userUrl = $"/users/{user.Id}";
        var userPayload = new
        {
            id = user.Id,
            pseudo = user.Pseudo,
            dateOfCreation = user.DateOfCreation
        };

        await SendPutRequest(client, baseUri, userUrl, userPayload);
    }

    private async Task UpdateUnit(HttpClient client, string baseUri, string userId, Unit unit)
    {
        var unitUrl = $"/users/{userId}/units/{unit.Id}";
        var unitPayload = new
        {
            id = unit.Id,
            type = unit.Type.ToString().ToLower(),
            health = unit.Health,
            resourcesQuantity = unit.ResourcesQuantity.ToDictionary(
                kvp => kvp.Key.ToString().ToLower(),
                kvp => kvp.Value
            )
        };

        await SendPutRequest(client, baseUri, unitUrl, unitPayload);
    }

    private async Task SendPutRequest(HttpClient client, string baseUri, string url, object payload)
    {
        var fullUrl = BuildFullRequestPath(baseUri, url);
        await client.PutAsJsonAsync(fullUrl, payload);
    }

    private static string BuildFullRequestPath(string baseUri, string request)
        => $"{baseUri}{request}";

    private static AuthenticationHeaderValue CreateAuthorizationHeader(string shardName, string sharedKey)
        => new(AuthenticationScheme, Convert.ToBase64String(Encoding.UTF8.GetBytes($"shard-{shardName}:{sharedKey}")));
}