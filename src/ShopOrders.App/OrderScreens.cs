namespace ShopOrders.App;

using ShopOrders.Application;
using ShopOrders.Application.Abstractions;
using ShopOrders.Application.Input;
using ShopOrders.Domain;

/// <summary>
/// Екрани сценаріїв, що змінюють замовлення: UC-01 — UC-04 і перегляд замовлення.
/// </summary>
public sealed class OrderScreens
{
    private readonly ConsoleUi _ui;
    private readonly OrderService _service;
    private readonly OrderQueryService _queries;
    private readonly ICatalog _catalog;

    /// <summary>
    /// Створює екрани замовлень.
    /// </summary>
    /// <param name="ui">Інтерфейс вводу й виводу.</param>
    /// <param name="service">Сервіс сценаріїв UC-01 — UC-04.</param>
    /// <param name="queries">Сервіс читання замовлень.</param>
    /// <param name="catalog">Довідники для показу назв.</param>
    public OrderScreens(
        ConsoleUi ui,
        OrderService service,
        OrderQueryService queries,
        ICatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(ui);
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(queries);
        ArgumentNullException.ThrowIfNull(catalog);
        _ui = ui;
        _service = service;
        _queries = queries;
        _catalog = catalog;
    }

    /// <summary>
    /// UC-01. Оформлення замовлення.
    /// </summary>
    public void Place()
    {
        int customerId = _ui.Ask("Номер покупця: ", InputParser.ParsePositiveNumber);
        Customer? customer = _catalog.FindCustomer(customerId);
        if (customer is null)
        {
            _ui.WriteLine($"Покупця №{customerId} не знайдено.");
            return;
        }

        _ui.WriteLine($"Покупець: {customer.FullName}, статус: {Display.Status(customer.Status)}");
        List<OrderLineRequest> lines = ReadLines();
        if (lines.Count == 0)
        {
            _ui.WriteLine("Не введено жодної позиції, замовлення не створено.");
            return;
        }

        Order order = _service.PlaceOrder(new PlaceOrderRequest(customerId, lines));
        _ui.WriteLine("Замовлення оформлено.");
        OrderPrinter.Print(_ui, order, _catalog);
    }

    /// <summary>
    /// UC-02. Оплата замовлення.
    /// </summary>
    public void Pay()
    {
        Order? order = AskAndPrintOrder();
        if (order is null)
        {
            return;
        }

        decimal amount = _ui.Ask("Внесена сума, грн: ", InputParser.ParseMoney);
        decimal change = _service.Pay(order.Id, amount);
        _ui.WriteLine($"Замовлення №{order.Id} оплачено. Решта: {Display.Money(change)} грн.");
    }

    /// <summary>
    /// UC-03. Скасування замовлення.
    /// </summary>
    public void Cancel()
    {
        int orderId = AskOrderId();
        _service.Cancel(orderId);
        _ui.WriteLine($"Замовлення №{orderId} скасовано.");
    }

    /// <summary>
    /// UC-04. Відвантаження замовлення.
    /// </summary>
    public void Ship()
    {
        int orderId = AskOrderId();
        _service.Ship(orderId);
        _ui.WriteLine($"Замовлення №{orderId} відвантажено.");
    }

    /// <summary>
    /// Перегляд складу замовлення.
    /// </summary>
    public void Show()
    {
        _ = AskAndPrintOrder();
    }

    private int AskOrderId()
    {
        return _ui.Ask("Номер замовлення: ", InputParser.ParsePositiveNumber);
    }

    private Order? AskAndPrintOrder()
    {
        int orderId = AskOrderId();
        Order? order = _queries.FindById(orderId);
        if (order is null)
        {
            _ui.WriteLine($"Замовлення №{orderId} не знайдено.");
            return null;
        }

        OrderPrinter.Print(_ui, order, _catalog);
        return order;
    }

    private List<OrderLineRequest> ReadLines()
    {
        List<OrderLineRequest> lines = new();
        _ui.WriteLine("Вводьте позиції; порожній артикул завершує ввід.");
        while (true)
        {
            string? text = _ui.ReadLine("Артикул: ");
            if (string.IsNullOrWhiteSpace(text))
            {
                return lines;
            }

            Product? product = FindProduct(text);
            if (product is not null)
            {
                int quantity = _ui.Ask("Кількість: ", InputParser.ParsePositiveNumber);
                lines.Add(new OrderLineRequest(product.Sku, quantity));
                string price = Display.Money(product.Price);
                _ui.WriteLine($"Додано: {product.Title}, {quantity} шт. × {price} грн");
            }
        }
    }

    private Product? FindProduct(string text)
    {
        (ValidationResult result, string sku) = InputParser.ParseSku(text);
        if (!result.IsValid)
        {
            _ui.WriteLine(result.ErrorMessage);
            return null;
        }

        Product? product = _catalog.FindProduct(sku);
        if (product is null)
        {
            _ui.WriteLine($"Товару {sku} немає в каталозі.");
        }

        return product;
    }
}
