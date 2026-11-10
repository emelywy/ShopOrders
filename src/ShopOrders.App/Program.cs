using System.Text;
using System.Text.Json;
using ShopOrders.App;
using ShopOrders.Application;
using ShopOrders.Application.Pricing;
using ShopOrders.Infrastructure;

// Композиційний корінь: єдине місце, де створюються конкретні реалізації.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
string dataDirectory = args.Length > 0 ? args[0] : "data";
SystemClock clock = new();
FileAppLog log = new(Path.Combine("logs", "shoporders.log"), clock);
try
{
    JsonCatalog catalog = JsonCatalog.Load(dataDirectory);
    JsonOrderRepository orders = JsonOrderRepository.Open(dataDirectory);
    OperationContext context = new(clock, log);
    OrderService service = new(orders, catalog, new TieredDiscountPolicy(), context);
    OrderQueryService queries = new(orders, log);
    ConsoleUi ui = new(Console.In, Console.Out);
    OrderScreens orderScreens = new(ui, service, queries, catalog);
    MainMenu menu = new(ui, orderScreens, new ReportScreens(ui, queries), log);
    return menu.Run();
}
catch (IOException ex)
{
    return Fail(ex);
}
catch (JsonException ex)
{
    return Fail(ex);
}

int Fail(Exception exception)
{
    log.LogError("startup", $"Не вдалося завантажити дані з теки {dataDirectory}.", exception);
    Console.Error.WriteLine($"Не вдалося завантажити дані: {exception.Message}");
    return 1;
}
