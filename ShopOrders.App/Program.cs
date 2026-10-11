using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Services;

var repository = new InMemoryOrderRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
var service = new OrderService(repository, pricing);

var order = new Order(1, 100, DateOnly.FromDateTime(DateTime.Today));
order.AddLine(new OrderLine("SKU-1", 2, 250m));

service.Place(order);

Console.WriteLine($"Сума: {service.TotalOf(order):0.00}");
