namespace ShopOrders.Application.Abstractions;

/// <summary>
/// Джерело поточного часу; у тестах підміняється фіксованим годинником.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Поточний момент часу з урахуванням часового поясу.
    /// </summary>
    DateTimeOffset Now { get; }
}
