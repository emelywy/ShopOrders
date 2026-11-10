namespace ShopOrders.Tests;

using ShopOrders.Application;
using ShopOrders.Application.Pricing;
using ShopOrders.Domain;
using ShopOrders.Tests.Doubles;

/// <summary>
/// Сценарії-ядра UC-01 — UC-04 на тестових дублерах: фейкове сховище,
/// фейкові довідники, фіксований годинник і шпигун журналу.
/// </summary>
public sealed class OrderServiceTests
{
    private readonly FakeOrderRepository _orders = new();
    private readonly SpyLog _log = new();
    private readonly FixedClock _clock = new(new DateTimeOffset(2026, 11, 10, 12, 0, 0, default));
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        OperationContext context = new(_clock, _log);
        TieredDiscountPolicy policy = new();
        _service = new OrderService(_orders, new FakeCatalog(), policy, context);
    }

    [Fact]
    public void PlaceOrder_LoyalCustomerMediumSum_AppliesFivePercent()
    {
        PlaceOrderRequest request = Request(
            FakeCatalog.LoyalCustomerId,
            new OrderLineRequest("GRINDER", 1),
            new OrderLineRequest("coffee", 2));

        Order order = _service.PlaceOrder(request);

        Assert.Equal(1500m, order.Subtotal);
        Assert.Equal(75m, order.Discount);
        Assert.Equal(1425m, order.Total);
        Assert.Equal(OrderStatus.New, order.Status);
        Assert.Equal(new DateOnly(2026, 11, 10), order.CreatedAt);
        Assert.Same(order, _orders.FindById(order.Id));
        Assert.Equal(1, _orders.SaveCount);
    }

    [Fact]
    public void PlaceOrder_StandardCustomerSmallSum_HasNoDiscount()
    {
        Order order = PlaceCoffee(FakeCatalog.StandardCustomerId, 1);

        Assert.Equal(0m, order.Discount);
        Assert.Equal(250m, order.Total);
    }

    [Fact]
    public void PlaceOrder_UnknownCustomer_ThrowsAndLogsWarning()
    {
        PlaceOrderRequest request = Request(99, new OrderLineRequest("COFFEE", 1));

        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.PlaceOrder(request));

        Assert.Equal(RuleCodes.CustomerNotFound, error.Rule);
        Assert.Equal(1, _log.WarningCount);
        Assert.Empty(_orders.ListAll());
    }

    [Fact]
    public void PlaceOrder_UnknownSku_Throws()
    {
        PlaceOrderRequest request = Request(
            FakeCatalog.StandardCustomerId,
            new OrderLineRequest("NO-SUCH", 1));

        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.PlaceOrder(request));

        Assert.Equal(RuleCodes.ProductNotFound, error.Rule);
        Assert.Empty(_orders.ListAll());
    }

    [Fact]
    public void PlaceOrder_NoLines_Throws()
    {
        PlaceOrderRequest request = Request(FakeCatalog.StandardCustomerId);

        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.PlaceOrder(request));

        Assert.Equal(RuleCodes.EmptyOrder, error.Rule);
    }

    [Fact]
    public void Pay_EnoughAmount_ReturnsChangeAndSetsPaidAt()
    {
        Order order = PlaceCoffee(FakeCatalog.StandardCustomerId, 1);

        decimal change = _service.Pay(order.Id, 300m);

        Assert.Equal(50m, change);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(_clock.Now, order.PaidAt);
    }

    [Fact]
    public void Pay_InsufficientAmount_ThrowsAndLogsWarning()
    {
        Order order = PlaceCoffee(FakeCatalog.StandardCustomerId, 1);

        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.Pay(order.Id, 100m));

        Assert.Equal(RuleCodes.InsufficientPayment, error.Rule);
        Assert.Equal(OrderStatus.New, order.Status);
        Assert.Equal(1, _log.WarningCount);
    }

    [Fact]
    public void Pay_UnknownOrder_Throws()
    {
        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.Pay(404, 100m));

        Assert.Equal(RuleCodes.OrderNotFound, error.Rule);
    }

    [Fact]
    public void Pay_AlreadyPaidOrder_ThrowsForbiddenTransition()
    {
        Order order = PlaceCoffee(FakeCatalog.StandardCustomerId, 1);
        _service.Pay(order.Id, 250m);

        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.Pay(order.Id, 250m));

        Assert.Equal(RuleCodes.ForbiddenTransition, error.Rule);
    }

    [Fact]
    public void Cancel_NewOrder_BecomesCancelled()
    {
        Order order = PlaceCoffee(FakeCatalog.VipCustomerId, 1);

        _service.Cancel(order.Id);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Contains(
            _log.Entries,
            entry => entry.StartsWith("INFO UC-03", StringComparison.Ordinal));
    }

    [Fact]
    public void Cancel_ShippedOrder_ThrowsAndKeepsState()
    {
        Order order = PlaceCoffee(FakeCatalog.StandardCustomerId, 1);
        _service.Pay(order.Id, 250m);
        _service.Ship(order.Id);

        Assert.Throws<DomainRuleException>(() => _service.Cancel(order.Id));

        Assert.Equal(OrderStatus.Shipped, order.Status);
        Assert.Equal(1, _log.WarningCount);
    }

    [Fact]
    public void Ship_PaidOrder_BecomesShipped()
    {
        Order order = PlaceCoffee(FakeCatalog.LoyalCustomerId, 4);
        _service.Pay(order.Id, order.Total);

        _service.Ship(order.Id);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public void Ship_NewOrder_Throws()
    {
        Order order = PlaceCoffee(FakeCatalog.StandardCustomerId, 1);

        DomainRuleException error =
            Assert.Throws<DomainRuleException>(() => _service.Ship(order.Id));

        Assert.Equal(RuleCodes.ForbiddenTransition, error.Rule);
        Assert.Equal(OrderStatus.New, order.Status);
    }

    private static PlaceOrderRequest Request(int customerId, params OrderLineRequest[] lines)
    {
        return new PlaceOrderRequest(customerId, lines);
    }

    private Order PlaceCoffee(int customerId, int quantity)
    {
        return _service.PlaceOrder(Request(customerId, new OrderLineRequest("COFFEE", quantity)));
    }
}
