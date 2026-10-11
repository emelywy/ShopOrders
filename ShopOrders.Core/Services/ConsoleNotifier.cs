namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;

/// <summary>
/// Сповіщувач, який виводить повідомлення в консоль.
/// </summary>
public sealed class ConsoleNotifier : INotifier
{
    /// <inheritdoc/>
    public void Notify(string recipient, string message)
    {
        Console.WriteLine($"[Сповіщення для {recipient}]: {message}");
    }
}
