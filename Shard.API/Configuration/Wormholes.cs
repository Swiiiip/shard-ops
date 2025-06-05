namespace Shard.API.Configuration;

public class Wormholes : Dictionary<string, Server>
{
    public new Server this[string? serverName]
    {
        get
        {
            if (serverName == null || !ContainsKey(serverName))
                throw new KeyNotFoundException($"The server '{serverName}' was not found.");
            
            return base[serverName];
        }
    }
}

public class Server
{
    public string BaseUri { get; init; }
    public string System { get; init; }
    public string User { get; init; }
    public string SharedPassword { get; init; }
}