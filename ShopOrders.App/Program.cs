using System.Text;
using Microsoft.Extensions.Logging;
using ShopOrders.App;
using ShopOrders.Core;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Errors;
using ShopOrders.Core.Reporting;
using ShopOrders.Core.Services;

Console.WriteLine($"ShopOrders v{VersionInfo.Current()}");
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

ILogger log = factory.CreateLogger("Program");

try
{
    RunApplication(factory);
    return 0;
}
catch (DomainRuleException ex)
{
    log.LogWarning("Правило {Rule}: {Message}", ex.Rule, ex.Message);
    return 1;
}
catch (Exception ex)
{
    log.LogCritical(ex, "Непередбачений збій застосунку");
    return 2;
}

static void RunApplication(ILoggerFactory factory)
{
    InMemoryOrderRepository repository = new InMemoryOrderRepository();
    DiscountPricingPolicy pricing = new DiscountPricingPolicy(0.10m);
    ConsoleNotifier notifier = new ConsoleNotifier();
    ILogger<OrderService> orderLogger = factory.CreateLogger<OrderService>();

    OrderService service = new OrderService(repository, pricing, notifier, orderLogger);

    Order order = new Order(1042, 101, DateOnly.FromDateTime(DateTime.Today));
    repository.Add(order);

    // Використовуємо НОВИЙ безпечний парсер із ShopOrders.Core
    if (ShopOrders.Core.OrderLineParser.TryParse("BOOK-01;2;149.50", out var parsedLine))
    {
        service.AddLine(order.Id, parsedLine!);
    }

    // Демонстрація логування оплати з Warning та Information
    service.Pay(order.Id, 300.00m);
    
    // Демонстрація безпечного файлового аудиту через using
    using (FileAuditLog audit = new("audit.log"))
    {
        audit.Write("ADD_LINE", order.Id);
        audit.Write("PAY_ORDER", order.Id);
    } 
}
