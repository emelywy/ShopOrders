namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Orders;

/// <summary>
/// Прикладний сервіс для оформлення та розрахунку вартості замовлень.
/// </summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="OrderService"/>.
    /// </summary>
    /// <param name="repository">Сховище замовлень.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    /// <param name="notifier">Сервіс сповіщень.</param>
    public OrderService(IOrderRepository repository, IPricingPolicy pricing, INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>
    /// Зберігає замовлення та сповіщає клієнта.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    /// <param name="customerEmail">Електронна пошта для сповіщення.</param>
    public void Place(Order order, string customerEmail)
    {
        ArgumentNullException.ThrowIfNull(order);

        _repository.Add(order);
        decimal total = TotalOf(order);
        _notifier.Notify(customerEmail, $"Замовлення #{order.Id} оформлено на суму: {total:0.00}");
    }

    /// <summary>
    /// Розраховує загальну вартість замовлення за поточною політикою.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    /// <returns>Загальна вартість.</returns>
    public decimal TotalOf(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        decimal total = 0m;
        foreach (OrderLine line in order.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }

    /// <summary>
    /// Додає новий рядок до існуючого замовлення.
    /// </summary>
    /// <param name="orderId">Ідентифікатор замовлення.</param>
    /// <param name="line">Типізований рядок замовлення.</param>
    public void AddLine(int orderId, OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        Order order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено.");

        if (order.Status != OrderStatus.New)
        {
            throw new DomainRuleException("order.editable", "Редагувати можна лише нове замовлення.");
        }

        order.AddLine(line);
    }
}
