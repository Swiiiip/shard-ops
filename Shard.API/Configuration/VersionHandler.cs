namespace Shard.API.Configuration;

public static class VersionHandler
{
    public static string GetVersion()
    {
        return Environment.GetEnvironmentVariable("API_VERSION") ?? "";
    }
}