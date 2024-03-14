using Microsoft.Extensions.Logging;

namespace PseudoFTP.Client.Utils;

public static class LogHelper
{
    private static ILogger? _logger;

    public static void Init(LogLevel level)
    {
        if (_logger != null)
        {
            throw new InvalidOperationException("Logger is already initialized.");
        }

        _logger = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(level))
            .CreateLogger("PseudoFTP");
    }

    public static ILogger GetLogger()
    {
        if (_logger is null)
        {
            throw new InvalidOperationException("Logger is not initialized.");
        }

        return _logger;
    }
}