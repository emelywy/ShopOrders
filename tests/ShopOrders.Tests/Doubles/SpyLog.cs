namespace ShopOrders.Tests.Doubles;

using ShopOrders.Application.Abstractions;

/// <summary>
/// Шпигун журналу: запам'ятовує записи, щоб тест перевірив рівень і сценарій.
/// </summary>
public sealed class SpyLog : IAppLog
{
    private readonly List<string> _entries = new();

    public IReadOnlyList<string> Entries => _entries;

    public int WarningCount =>
        _entries.Count(entry => entry.StartsWith("WARN", StringComparison.Ordinal));

    public void LogInformation(string scenario, string message)
    {
        _entries.Add($"INFO {scenario} {message}");
    }

    public void LogWarning(string scenario, string message)
    {
        _entries.Add($"WARN {scenario} {message}");
    }

    public void LogError(string scenario, string message, Exception exception)
    {
        _entries.Add($"ERROR {scenario} {message} {exception.Message}");
    }
}
