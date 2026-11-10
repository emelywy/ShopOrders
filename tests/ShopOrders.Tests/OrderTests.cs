namespace ShopOrders.Tests;

using ShopOrders.Domain;

/// <summary>
/// Інваріанти агрегату Order: зміна стану, позиції, знижка й оплата.
/// </summary>
public sealed class OrderTests
{
    private static DateTimeOffset PaidMoment => new(2026, 10, 2, 10, 0, 0, TimeSpan.FromHours(3));

    [Fact]
    public void Pay_NewOrderWithEnoughAmount_BecomesPaidWithTime()
    {
        Order order = CreateOrder();

        order.Pay(500m, PaidMoment);

        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(PaidMoment, order.PaidAt);
    }

    [Fact]
    public void Pay_PaidOrder_ThrowsForbiddenTransition()
    {
        Order order = CreateOrder();
        order.Pay(500m, PaidMoment);

        DomainRuleException error = Assert.Throws<DomainRuleException>(
            () => order.Pay(500m, PaidMoment));

        Assert.Equal(RuleCodes.ForbiddenTransition, error.Rule);
    }

    [Fact]
    public void Pay_InsufficientAmount_ThrowsAndStaysNew()
    {
        Order order = CreateOrder();

        DomainRuleException error = Assert.Throws<DomainRuleException>(
            () => order.Pay(499.99m, PaidMoment));

        Assert.Equal(RuleCodes.InsufficientPayment, error.Rule);
        Assert.Equal(OrderStatus.New, order.Status);
    }

    [Fact]
    public void Pay_EmptyOrder_ThrowsEmptyOrder()
    {
        Order order = new(1, 1, new DateOnly(2026, 10, 1));

        DomainRuleException error = Assert.Throws<DomainRuleException>(
            () => order.Pay(0m, PaidMoment));

        Assert.Equal(RuleCodes.EmptyOrder, error.Rule);
    }

    [Fact]
    public void Ship_NewOrder_ThrowsForbiddenTransition()
    {
        Order order = CreateOrder();

        DomainRuleException error = Assert.Throws<DomainRuleException>(order.Ship);

        Assert.Equal(RuleCodes.ForbiddenTransition, error.Rule);
        Assert.Equal(OrderStatus.New, order.Status);
    }

    [Fact]
    public void Cancel_PaidOrder_BecomesCancelled()
    {
        Order order = CreateOrder();
        order.Pay(500m, PaidMoment);

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ShippedOrder_ThrowsAndKeepsShipped()
    {
        Order order = CreateOrder();
        order.Pay(500m, PaidMoment);
        order.Ship();

        Assert.Throws<DomainRuleException>(order.Cancel);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void AddLine_PaidOrder_ThrowsNotEditable()
    {
        Order order = CreateOrder();
        order.Pay(500m, PaidMoment);

        DomainRuleException error = Assert.Throws<DomainRuleException>(
            () => order.AddLine(new OrderLine("TEA", 1, 100m)));

        Assert.Equal(RuleCodes.NotEditable, error.Rule);
    }

    [Fact]
    public void ApplyDiscount_GreaterThanSubtotal_ThrowsArgumentOutOfRange()
    {
        Order order = CreateOrder();

        Assert.Throws<ArgumentOutOfRangeException>(() => order.ApplyDiscount(500.01m));
    }

    [Fact]
    public void Total_TwoLinesWithDiscount_IsSubtotalMinusDiscount()
    {
        Order order = CreateOrder();
        order.AddLine(new OrderLine("TEA", 3, 33.33m));

        order.ApplyDiscount(18m);

        Assert.Equal(599.99m, order.Subtotal);
        Assert.Equal(581.99m, order.Total);
    }

    private static Order CreateOrder()
    {
        Order order = new(1, 1, new DateOnly(2026, 10, 1));
        order.AddLine(new OrderLine("COFFEE", 2, 250m));
        return order;
    }
}
