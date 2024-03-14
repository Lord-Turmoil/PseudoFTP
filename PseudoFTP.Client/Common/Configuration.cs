using Newtonsoft.Json;

namespace PseudoFTP.Client.Common;

class Configuration
{
    public string Server { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int MaxTimeout { get; set; }
    public int MaxRetry { get; set; }

    public static Configuration Load(string path)
    {
        if (!File.Exists(path))
        {
            Console.Error.WriteLine("Configuration file not found, expecting overrides from commandline.");
            return new Configuration {
                Server = "http://localhost",
                Username = "_",
                Password = "_",
                MaxTimeout = 1000,
                MaxRetry = 10
            };
        }

        using StreamReader reader = new(path);
        string json = reader.ReadToEnd();
        Configuration config = JsonConvert.DeserializeObject<Configuration>(json) ??
                               throw new Exception("Invalid configuration");
        return config;
    }
}