namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;

/// <summary>
/// Реалізація сховища замовлень у пам'яті на основі словника.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _orders = new();

    /// <inheritdoc/>
    public void Add(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Замовлення з ідентифікатором {order.Id} вже існує у сховищі.");
        }
    }

    /// <inheritdoc/>
    public Order? GetById(int id)
    {
        _orders.TryGetValue(id, out Order? order);
        return order;
    }

    /// <inheritdoc/>
    public IReadOnlyList<Order> GetAll()
    {
        return _orders.Values.ToList().AsReadOnly();
    }
}
