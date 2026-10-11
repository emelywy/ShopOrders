using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Services;

// 1. Налаштування політик через словник (OCP замість if/else)
string customerType = "vip";

var policies = new Dictionary<string, IPricingPolicy>
{
    ["standard"] = new StandardPricingPolicy(),
    ["regular"] = new DiscountPricingPolicy(0.05m),
    ["vip"] = new VipPricingPolicy(),
    ["staff"] = new DiscountPricingPolicy(0.30m),
};

IPricingPolicy selectedPolicy = policies.TryGetValue(customerType, out var policy)
    ? policy
    : new StandardPricingPolicy();

// 2. Збирання залежностей (DIP без контейнера)
var repository = new InMemoryOrderRepository();
var notifier = new ConsoleNotifier();
var service = new OrderService(repository, selectedPolicy, notifier);

// 3. Робота з агрегатом
var order = new Order(1, 100, DateOnly.FromDateTime(DateTime.Today));
order.AddLine(new OrderLine("SKU-1", 2, 250m));

service.Place(order, "client@example.com");

Console.WriteLine($"Сума: {service.TotalOf(order):0.00}");

// 4. Демонстрація LSP (композиція для архіву)
var archived = new ArchivedOrder(order, DateOnly.FromDateTime(DateTime.Today));
Console.WriteLine($"Замовлення #{archived.Id} успішно заархівовано.");

// 5. Демонстрація SRP (звіт окремо)
string csv = OrderCsvReport.BuildCsv(repository.GetAll());
Console.WriteLine("CSV звіт:");
Console.Write(csv);
