using Serilog;

namespace CertificateManager.DbManager.Features;

public class Logging
{
    public static ILogger CreateLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
    }
}