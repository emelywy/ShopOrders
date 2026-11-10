namespace ShopOrders.Tests.Doubles;

using ShopOrders.Application.Abstractions;
using ShopOrders.Domain;

/// <summary>
/// Фейкові довідники з кількома товарами й покупцями всіх статусів.
/// </summary>
public sealed class FakeCatalog : ICatalog
{
    public const int StandardCustomerId = 1;
    public const int LoyalCustomerId = 2;
    public const int VipCustomerId = 3;

    private readonly Dictionary<string, Product> _products = new(StringComparer.OrdinalIgnoreCase)
    {
        ["COFFEE"] = new Product("COFFEE", "Кава мелена", 250m),
        ["GRINDER"] = new Product("GRINDER", "Кавомолка", 1000m),
    };

    private readonly Dictionary<int, Customer> _customers = new()
    {
        [StandardCustomerId] = new Customer(StandardCustomerId, "Тест 1", CustomerStatus.Standard),
        [LoyalCustomerId] = new Customer(LoyalCustomerId, "Тест 2", CustomerStatus.Loyal),
        [VipCustomerId] = new Customer(VipCustomerId, "Тест 3", CustomerStatus.Vip),
    };

    public Product? FindProduct(string sku)
    {
        return _products.TryGetValue(sku, out Product? product) ? product : null;
    }

    public Customer? FindCustomer(int id)
    {
        return _customers.TryGetValue(id, out Customer? customer) ? customer : null;
    }
}
