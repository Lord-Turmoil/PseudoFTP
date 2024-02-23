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
        using StreamReader reader = new(path);
        string json = reader.ReadToEnd();
        Configuration config = JsonConvert.DeserializeObject<Configuration>(json) ??
                        throw new Exception("Invalid configuration");
        return config;
    }
}