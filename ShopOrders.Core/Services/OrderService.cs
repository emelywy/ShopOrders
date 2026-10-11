namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;

/// <summary>
/// Прикладний сервіс для оформлення та розрахунку вартості замовлень.
/// </summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="OrderService"/>.
    /// </summary>
    /// <param name="repository">Сховище замовлень.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    public OrderService(IOrderRepository repository, IPricingPolicy pricing)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    /// <summary>
    /// Зберігає замовлення в системі.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    public void Place(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _repository.Add(order);
    }

    /// <summary>
    /// Розраховує загальну вартість замовлення за обраною політикою.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    /// <returns>Загальна вартість замовлення.</returns>
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
}
