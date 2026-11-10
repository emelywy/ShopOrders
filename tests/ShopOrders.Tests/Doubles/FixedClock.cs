namespace ShopOrders.Tests.Doubles;

using ShopOrders.Application.Abstractions;

/// <summary>
/// Фіксований годинник замість системного: тести не залежать від дати запуску.
/// </summary>
public sealed class FixedClock : IClock
{
    public FixedClock(DateTimeOffset now)
    {
        Now = now;
    }

    public DateTimeOffset Now { get; }
}
