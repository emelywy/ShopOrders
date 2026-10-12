using System.Text;
using ShopOrders.App;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Errors;
using ShopOrders.Core.Services;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.WriteLine("Демонстрація безпечного вводу");

InMemoryOrderRepository repository = new InMemoryOrderRepository();
DiscountPricingPolicy pricing = new DiscountPricingPolicy(0.10m);
ConsoleNotifier notifier = new ConsoleNotifier();
OrderService service = new OrderService(repository, pricing, notifier);

// Створюємо демонстраційне замовлення
Order order = new Order(1, 101, DateOnly.FromDateTime(DateTime.Today));
repository.Add(order);

while (true)
{
    Console.WriteLine("\nВведіть дані позиції замовлення:");
    Console.Write("Артикул (SKU) (або 'exit' для завершення): ");
    string? sku = Console.ReadLine();
    if (string.Equals(sku, "exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Console.Write("Кількість: ");
    string? qty = Console.ReadLine();

    Console.Write("Ціна: ");
    string? price = Console.ReadLine();

    Result<OrderLine> parsed = OrderLineParser.Parse(sku ?? string.Empty, qty ?? string.Empty, price ?? string.Empty);
    if (!parsed.IsSuccess)
    {
        Console.WriteLine($"Помилка вводу: {parsed.Error}");
        Console.WriteLine("Будь ласка, повторіть введення з коректними даними.");
        continue;
    }

    service.AddLine(order.Id, parsed.Value!);
    Console.WriteLine($"Позицію успішно додано! Загальна кількість позицій у замовленні: {order.Lines.Count}");
    break;
}
