namespace DefaultNamespace;

public static class VersionHandler
{
    public static string GetVersion()
    {
        return Environment.GetEnvironmentVariable("API_VERSION") ?? "Development";
    }
}