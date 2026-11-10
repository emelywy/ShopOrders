namespace ShopOrders.Application;

using System.Globalization;
using ShopOrders.Application.Abstractions;
using ShopOrders.Application.Pricing;
using ShopOrders.Domain;

/// <summary>
/// Прикладний сервіс сценаріїв, що змінюють замовлення: UC-01 — UC-04.
/// </summary>
public sealed class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICatalog _catalog;
    private readonly IDiscountPolicy _discounts;
    private readonly IClock _clock;
    private readonly IAppLog _log;

    /// <summary>
    /// Створює сервіс; усі залежності передаються ззовні з композиційного кореня.
    /// </summary>
    /// <param name="orders">Сховище замовлень.</param>
    /// <param name="catalog">Довідники товарів і покупців.</param>
    /// <param name="discounts">Політика знижки.</param>
    /// <param name="context">Годинник і журнал.</param>
    public OrderService(
        IOrderRepository orders,
        ICatalog catalog,
        IDiscountPolicy discounts,
        OperationContext context)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(discounts);
        ArgumentNullException.ThrowIfNull(context);
        _orders = orders;
        _catalog = catalog;
        _discounts = discounts;
        _clock = context.Clock;
        _log = context.Log;
    }

    /// <summary>
    /// UC-01. Оформлює замовлення: бере ціни з каталогу, фіксує знижку й зберігає.
    /// </summary>
    /// <param name="request">Покупець і позиції замовлення.</param>
    /// <returns>Збережене замовлення в стані New.</returns>
    /// <exception cref="DomainRuleException">
    /// Покупця чи товару не знайдено або немає жодної позиції.
    /// </exception>
    public Order PlaceOrder(PlaceOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _log.LogInformation(Scenarios.PlaceOrder, $"Початок: покупець №{request.CustomerId}.");
        Customer customer = _catalog.FindCustomer(request.CustomerId)
            ?? throw Reject(
                Scenarios.PlaceOrder,
                RuleCodes.CustomerNotFound,
                $"Покупця №{request.CustomerId} не знайдено.");
        if (request.Lines.Count == 0)
        {
            throw Reject(Scenarios.PlaceOrder, RuleCodes.EmptyOrder, "Не введено жодної позиції.");
        }

        Order order = new(_orders.NextId(), customer.Id, DateOnly.FromDateTime(_clock.Now.Date));
        foreach (OrderLineRequest line in request.Lines)
        {
            order.AddLine(BuildLine(line));
        }

        order.ApplyDiscount(_discounts.DiscountFor(order.Subtotal, customer.Status));
        _orders.Add(order);
        _orders.SaveChanges();
        string summary = string.Create(
            CultureInfo.InvariantCulture,
            $"Завершено: замовлення №{order.Id}, до сплати {order.Total:0.00} грн.");
        _log.LogInformation(Scenarios.PlaceOrder, summary);
        return order;
    }

    /// <summary>
    /// UC-02. Оплачує замовлення.
    /// </summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <param name="amount">Внесена сума, грн.</param>
    /// <returns>Решта, яку слід повернути покупцю, грн.</returns>
    /// <exception cref="DomainRuleException">
    /// Замовлення не знайдено, оплата заборонена в його стані або суми недостатньо.
    /// </exception>
    public decimal Pay(int orderId, decimal amount)
    {
        _log.LogInformation(Scenarios.PayOrder, $"Початок: замовлення №{orderId}.");
        Order order = Require(Scenarios.PayOrder, orderId);
        Apply(Scenarios.PayOrder, () => order.Pay(amount, _clock.Now));
        _orders.SaveChanges();
        _log.LogInformation(Scenarios.PayOrder, $"Завершено: замовлення №{orderId} оплачено.");
        return amount - order.Total;
    }

    /// <summary>
    /// UC-03. Скасовує замовлення, яке ще не відвантажено.
    /// </summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <exception cref="DomainRuleException">
    /// Замовлення не знайдено або скасування заборонене в його стані.
    /// </exception>
    public void Cancel(int orderId)
    {
        ChangeState(Scenarios.CancelOrder, orderId, order => order.Cancel());
    }

    /// <summary>
    /// UC-04. Відвантажує оплачене замовлення.
    /// </summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <exception cref="DomainRuleException">
    /// Замовлення не знайдено або відвантаження заборонене в його стані.
    /// </exception>
    public void Ship(int orderId)
    {
        ChangeState(Scenarios.ShipOrder, orderId, order => order.Ship());
    }

    private void ChangeState(string scenario, int orderId, Action<Order> transition)
    {
        _log.LogInformation(scenario, $"Початок: замовлення №{orderId}.");
        Order order = Require(scenario, orderId);
        Apply(scenario, () => transition(order));
        _orders.SaveChanges();
        _log.LogInformation(scenario, $"Завершено: замовлення №{orderId} у стані {order.Status}.");
    }

    private OrderLine BuildLine(OrderLineRequest line)
    {
        Product product = _catalog.FindProduct(line.Sku)
            ?? throw Reject(
                Scenarios.PlaceOrder,
                RuleCodes.ProductNotFound,
                $"Товару {line.Sku} немає в каталозі.");
        return new OrderLine(product.Sku, line.Quantity, product.Price);
    }

    private Order Require(string scenario, int orderId)
    {
        return _orders.FindById(orderId)
            ?? throw Reject(
                scenario,
                RuleCodes.OrderNotFound,
                $"Замовлення №{orderId} не знайдено.");
    }

    private void Apply(string scenario, Action transition)
    {
        try
        {
            transition();
        }
        catch (DomainRuleException ex)
        {
            _log.LogWarning(scenario, $"Відхилено ({ex.Rule}): {ex.Message}");
            throw;
        }
    }

    private DomainRuleException Reject(string scenario, string rule, string message)
    {
        _log.LogWarning(scenario, $"Відхилено ({rule}): {message}");
        return new DomainRuleException(rule, message);
    }
}
