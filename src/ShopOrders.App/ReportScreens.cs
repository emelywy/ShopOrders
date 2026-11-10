namespace ShopOrders.App;

using ShopOrders.Application;
using ShopOrders.Application.Input;
using ShopOrders.Application.Reports;
using ShopOrders.Domain;

/// <summary>
/// Екрани сценаріїв, що лише читають дані: UC-05 і UC-06.
/// </summary>
public sealed class ReportScreens
{
    private readonly ConsoleUi _ui;
    private readonly OrderQueryService _queries;

    /// <summary>
    /// Створює екрани пошуку й звітів.
    /// </summary>
    /// <param name="ui">Інтерфейс вводу й виводу.</param>
    /// <param name="queries">Сервіс пошуку й звітів.</param>
    public ReportScreens(ConsoleUi ui, OrderQueryService queries)
    {
        ArgumentNullException.ThrowIfNull(ui);
        ArgumentNullException.ThrowIfNull(queries);
        _ui = ui;
        _queries = queries;
    }

    /// <summary>
    /// UC-05. Пошук замовлень за покупцем, станом і періодом створення.
    /// </summary>
    public void Search()
    {
        _ui.WriteLine("Порожній ввід означає «будь-яке значення».");
        OrderSearchCriteria criteria = new()
        {
            CustomerId = _ui.Ask(
                "Номер покупця: ",
                text => InputParser.ParseOptional(text, InputParser.ParsePositiveNumber)),
            Status = _ui.Ask(
                "Стан (New, Paid, Shipped, Cancelled): ",
                text => InputParser.ParseOptional(text, InputParser.ParseStatus)),
            CreatedFrom = _ui.Ask(
                "Створено з (дд.мм.рррр): ",
                text => InputParser.ParseOptional(text, InputParser.ParseDate)),
            CreatedTo = _ui.Ask(
                "Створено по (дд.мм.рррр): ",
                text => InputParser.ParseOptional(text, InputParser.ParseDate)),
        };
        IReadOnlyList<Order> found = _queries.Search(criteria);
        if (found.Count == 0)
        {
            _ui.WriteLine("Нічого не знайдено.");
            return;
        }

        _ui.WriteLine("   №  | Створено   | Покупець | Стан          | До сплати");
        foreach (Order order in found)
        {
            string status = Display.Status(order.Status);
            string line = $"{order.Id,5} | {Display.Date(order.CreatedAt)} | "
                + $"{order.CustomerId,8} | {status,-13} | {Display.Money(order.Total),10}";
            _ui.WriteLine(line);
        }

        _ui.WriteLine($"Знайдено замовлень: {found.Count}");
    }

    /// <summary>
    /// UC-06. Звіт «Продажі за період».
    /// </summary>
    public void SalesByPeriod()
    {
        DateOnly start = _ui.Ask("Початок періоду (дд.мм.рррр): ", InputParser.ParseDate);
        DateOnly end = _ui.Ask("Кінець періоду (дд.мм.рррр): ", InputParser.ParseDate);
        ValidationResult period = InputParser.ValidatePeriod(start, end);
        if (!period.IsValid)
        {
            _ui.WriteLine(period.ErrorMessage);
            return;
        }

        SalesReport report = _queries.BuildSalesReport(start, end);
        _ui.WriteLine($"Продажі за період {Display.Date(start)} – {Display.Date(end)}");
        if (report.IsEmpty)
        {
            _ui.WriteLine("Продажів за період немає.");
            return;
        }

        _ui.WriteLine("Дата       | Замовлень |      Сума |   Знижка |   Виручка");
        foreach (SalesReportRow row in report.Rows)
        {
            string amounts = FormatAmounts(row.Subtotal, row.Discount, row.Revenue);
            _ui.WriteLine($"{Display.Date(row.Date),-10} | {row.OrderCount,9} | {amounts}");
        }

        string total = FormatAmounts(report.Subtotal, report.Discount, report.Revenue);
        _ui.WriteLine($"{"Разом",-10} | {report.OrderCount,9} | {total}");
    }

    private static string FormatAmounts(decimal subtotal, decimal discount, decimal revenue)
    {
        return $"{Display.Money(subtotal),9} | {Display.Money(discount),8} | "
            + $"{Display.Money(revenue),9}";
    }
}
