using Microsoft.Extensions.Logging;

namespace HostelMealManagement.Application.Logging;

public interface IAppLogger<T>
{
    // Logs an informational message.
    void LogInfo(string message);
    // Logs a warning message.
    void LogWarning(string message);
    /// <summary>
    /// Logs an error message along with exception details to the configured error output or logging system.
    /// </summary>
    /// <remarks>Use this method to record errors that occur during execution, including both a descriptive
    /// message and exception information. The logging destination and format depend on the implementation of the
    /// logger.</remarks>
    /// <param name="message">The error message to log. This should describe the context or nature of the error.</param>
    /// <param name="ex">The exception associated with the error. Provides stack trace and exception details for diagnostic purposes.
    /// Cannot be null.</param>
    void LogError(string message, Exception ex);
}
public class AppLogger<T>(ILogger<T> logger) : IAppLogger<T>
{
    public void LogInfo(string message) =>
        logger.LogInformation(message);

    public void LogWarning(string message) =>
        logger.LogWarning(message);

    public void LogError(string message, Exception ex) =>
        logger.LogError(ex, message);
}
