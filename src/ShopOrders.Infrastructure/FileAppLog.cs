namespace ShopOrders.Infrastructure;

using System.Globalization;
using ShopOrders.Application.Abstractions;

/// <summary>
/// Журнал у текстовому файлі: рядок «час | рівень | сценарій | повідомлення».
/// </summary>
public sealed class FileAppLog : IAppLog
{
    /// <summary>
    /// Формат часу в рядку журналу.
    /// </summary>
    public const string TimeFormat = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// Роздільник полів рядка журналу.
    /// </summary>
    public const string Separator = " | ";

    private readonly string _filePath;
    private readonly IClock _clock;

    /// <summary>
    /// Створює журнал і, за потреби, теку для нього.
    /// </summary>
    /// <param name="filePath">Шлях до файлу журналу.</param>
    /// <param name="clock">Джерело часу для позначок у рядках.</param>
    public FileAppLog(string filePath, IClock clock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(clock);
        _filePath = filePath;
        _clock = clock;
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <inheritdoc/>
    public void LogInformation(string scenario, string message)
    {
        Write("INFO", scenario, message);
    }

    /// <inheritdoc/>
    public void LogWarning(string scenario, string message)
    {
        Write("WARN", scenario, message);
    }

    /// <inheritdoc/>
    public void LogError(string scenario, string message, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        string details = $"{exception.GetType().Name}: {exception.Message}";
        Write("ERROR", scenario, message + Separator + details);
    }

    private void Write(string level, string scenario, string message)
    {
        string time = _clock.Now.ToString(TimeFormat, CultureInfo.InvariantCulture);
        string line = string.Join(Separator, time, level, scenario, message);
        File.AppendAllText(_filePath, line + Environment.NewLine);
    }
}
