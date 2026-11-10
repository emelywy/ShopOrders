namespace ShopOrders.Tests;

using ShopOrders.Domain;

/// <summary>
/// Таблиця переходів 1.5: усі дозволені й усі заборонені пари «стан — подія».
/// </summary>
public sealed class OrderTransitionsTests
{
    [Theory]
    [InlineData(OrderStatus.New, OrderEvent.Pay, OrderStatus.Paid)]
    [InlineData(OrderStatus.New, OrderEvent.Cancel, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Paid, OrderEvent.Ship, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Paid, OrderEvent.Cancel, OrderStatus.Cancelled)]
    public void TargetOf_AllowedTransition_ReturnsNewStatus(
        OrderStatus current,
        OrderEvent orderEvent,
        OrderStatus expected)
    {
        OrderStatus? actual = OrderTransitions.TargetOf(current, orderEvent);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(OrderStatus.New, OrderEvent.Ship)]
    [InlineData(OrderStatus.Paid, OrderEvent.Pay)]
    [InlineData(OrderStatus.Shipped, OrderEvent.Pay)]
    [InlineData(OrderStatus.Shipped, OrderEvent.Ship)]
    [InlineData(OrderStatus.Shipped, OrderEvent.Cancel)]
    [InlineData(OrderStatus.Cancelled, OrderEvent.Pay)]
    [InlineData(OrderStatus.Cancelled, OrderEvent.Ship)]
    [InlineData(OrderStatus.Cancelled, OrderEvent.Cancel)]
    public void IsAllowed_ForbiddenTransition_ReturnsFalse(
        OrderStatus current,
        OrderEvent orderEvent)
    {
        bool allowed = OrderTransitions.IsAllowed(current, orderEvent);

        Assert.False(allowed);
    }
}
