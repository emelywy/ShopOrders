namespace ShopOrders.Application;

using ShopOrders.Application.Abstractions;

/// <summary>
/// Наскрізні залежності прикладних сервісів: годинник і журнал.
/// Об'єднані в один об'єкт, щоб конструктори сервісів мали не більше чотирьох параметрів.
/// </summary>
public sealed class OperationContext
{
    /// <summary>
    /// Створює контекст операцій.
    /// </summary>
    /// <param name="clock">Джерело поточного часу.</param>
    /// <param name="log">Журнал роботи застосунку.</param>
    public OperationContext(IClock clock, IAppLog log)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(log);
        Clock = clock;
        Log = log;
    }

    /// <summary>
    /// Джерело поточного часу.
    /// </summary>
    public IClock Clock { get; }

    /// <summary>
    /// Журнал роботи застосунку.
    /// </summary>
    public IAppLog Log { get; }
}
