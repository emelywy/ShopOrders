using System.Collections.Generic;
using System.Linq;
using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;

namespace ShopOrders.Tests;

/// <summary>Фейковий репозиторій у пам'яті.</summary>
internal sealed class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _items = new();

    /// <summary>Кількість викликів методу Save.</summary>
    public int SaveCalls { get; private set; }

    /// <summary>Додає замовлення.</summary>
    /// <param name="order">Замовлення для додавання.</param>
    public void Add(Order order) => _items[order.Id] = order;

    /// <summary>Повертає замовлення за ID.</summary>
    /// <param name="id">Ідентифікатор замовлення.</param>
    /// <returns>Знайдене замовлення або null.</returns>
    public Order? GetById(int id) => _items.TryGetValue(id, out Order? found) ? found : null;

    /// <summary>Повертає всі замовлення.</summary>
    /// <returns>Колекція замовлень.</returns>
    public IReadOnlyList<Order> GetAll() => _items.Values.ToList();

    /// <summary>Імітує збереження в базу.</summary>
    public void Save() => SaveCalls++;
}
