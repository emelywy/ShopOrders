namespace ShopOrders.Tests.Doubles;

using ShopOrders.Application.Abstractions;
using ShopOrders.Domain;

/// <summary>
/// Фейкове сховище в пам'яті замість файлового: працює по-справжньому, але без диска.
/// </summary>
public sealed class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _items = new();

    public int SaveCount { get; private set; }

    public int NextId()
    {
        return _items.Count + 1;
    }

    public void Add(Order order)
    {
        _items[order.Id] = order;
    }

    public Order? FindById(int id)
    {
        return _items.TryGetValue(id, out Order? order) ? order : null;
    }

    public IReadOnlyList<Order> ListAll()
    {
        return _items.Values.ToList();
    }

    public void SaveChanges()
    {
        SaveCount++;
    }
}
