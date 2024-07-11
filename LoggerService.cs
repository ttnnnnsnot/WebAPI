using Serilog;

namespace WebAPI;

public class LoggerService : ILoggerService
{
    private readonly Serilog.ILogger _logger;

    public LoggerService()
    {
        _logger = Log.Logger;
    }

    public void LogInformation(string message)
    {
        _logger.Information(message);
    }

    public void LogWarning(string message)
    {
        _logger.Warning(message);
    }

    public void LogError(string message, Exception exception)
    {
        _logger.Error(exception, message);
    }
}

