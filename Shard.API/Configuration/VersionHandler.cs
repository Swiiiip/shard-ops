using System.Text.Json;

namespace Shard.API.Configuration
{
    public static class VersionHandler
    {
        private class VersionInfo
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Patch { get; set; }
        }

        public static string GetVersion()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "version.json");
            VersionInfo version;

            if (!File.Exists(path))
            {
                version = new VersionInfo
                {
                    Year = DateTime.UtcNow.Year % 100,
                    Month = DateTime.UtcNow.Month,
                    Patch = 0
                };
            }
            else
            {
                version = JsonSerializer.Deserialize<VersionInfo>(File.ReadAllText(path))!;
                var now = DateTime.UtcNow;
                if (version.Year == now.Year % 100 && version.Month == now.Month)
                {
                    version.Patch += 1;
                }
                else
                {
                    version.Year = now.Year % 100;
                    version.Month = now.Month;
                    version.Patch = 0;
                }
            }

            File.WriteAllText(path, JsonSerializer.Serialize(version, new JsonSerializerOptions { WriteIndented = true }));
            return $"{version.Year}.{version.Month}.{version.Patch}";
        }
    }
}