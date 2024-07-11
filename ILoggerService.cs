namespace WebAPI;

public interface ILoggerService
{
    public void LogInformation(string message);
    public void LogWarning(string message);
    public void LogError(string message, Exception exception);
}