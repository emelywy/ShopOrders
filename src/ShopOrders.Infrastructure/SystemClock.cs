namespace ShopOrders.Infrastructure;

using ShopOrders.Application.Abstractions;

/// <summary>
/// Годинник, що повертає системний час комп'ютера.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc/>
    public DateTimeOffset Now => DateTimeOffset.Now;
}
