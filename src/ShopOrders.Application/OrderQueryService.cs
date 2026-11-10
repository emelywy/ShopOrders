namespace ShopOrders.Application;

using ShopOrders.Application.Abstractions;
using ShopOrders.Application.Reports;
using ShopOrders.Domain;

/// <summary>
/// Прикладний сервіс сценаріїв, що лише читають дані: UC-05 і UC-06.
/// </summary>
public sealed class OrderQueryService
{
    private readonly IOrderRepository _orders;
    private readonly IAppLog _log;

    /// <summary>
    /// Створює сервіс запитів.
    /// </summary>
    /// <param name="orders">Сховище замовлень.</param>
    /// <param name="log">Журнал роботи застосунку.</param>
    public OrderQueryService(IOrderRepository orders, IAppLog log)
    {
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(log);
        _orders = orders;
        _log = log;
    }

    /// <summary>
    /// Шукає замовлення за номером для показу оператору.
    /// </summary>
    /// <param name="orderId">Номер замовлення.</param>
    /// <returns>Замовлення або <see langword="null"/>.</returns>
    public Order? FindById(int orderId)
    {
        return _orders.FindById(orderId);
    }

    /// <summary>
    /// UC-05. Добирає замовлення за критеріями.
    /// </summary>
    /// <param name="criteria">Критерії пошуку; незадані не обмежують вибірку.</param>
    /// <returns>Замовлення, впорядковані за датою створення, потім за номером.</returns>
    public IReadOnlyList<Order> Search(OrderSearchCriteria criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        _log.LogInformation(Scenarios.SearchOrders, "Початок пошуку.");
        List<Order> found = _orders.ListAll()
            .Where(criteria.Matches)
            .OrderBy(order => order.CreatedAt)
            .ThenBy(order => order.Id)
            .ToList();
        _log.LogInformation(Scenarios.SearchOrders, $"Завершено: знайдено {found.Count}.");
        return found;
    }

    /// <summary>
    /// UC-06. Формує звіт «Продажі за період» за днями оплати.
    /// Продаж — замовлення в стані Paid або Shipped, оплачене в межах періоду.
    /// </summary>
    /// <param name="periodStart">Перший день періоду (включно).</param>
    /// <param name="periodEnd">Останній день періоду (включно).</param>
    /// <returns>Звіт із рядками, впорядкованими за датою.</returns>
    /// <exception cref="ArgumentException">Кінець періоду раніше за початок.</exception>
    public SalesReport BuildSalesReport(DateOnly periodStart, DateOnly periodEnd)
    {
        if (periodEnd < periodStart)
        {
            throw new ArgumentException("Кінець періоду раніше за початок.", nameof(periodEnd));
        }

        _log.LogInformation(Scenarios.SalesByPeriod, $"Початок: {periodStart} - {periodEnd}.");
        List<SalesReportRow> rows = _orders.ListAll()
            .Where(order => IsSale(order, periodStart, periodEnd))
            .GroupBy(PaidDate)
            .OrderBy(group => group.Key)
            .Select(group => new SalesReportRow(group.Key, group.ToList()))
            .ToList();
        _log.LogInformation(Scenarios.SalesByPeriod, $"Завершено: днів із продажами {rows.Count}.");
        return new SalesReport(periodStart, periodEnd, rows);
    }

    private static bool IsSale(Order order, DateOnly periodStart, DateOnly periodEnd)
    {
        if (order.PaidAt is null || order.Status == OrderStatus.Cancelled)
        {
            return false;
        }

        DateOnly paidOn = PaidDate(order);
        return paidOn >= periodStart && paidOn <= periodEnd;
    }

    private static DateOnly PaidDate(Order order)
    {
        return DateOnly.FromDateTime(order.PaidAt.GetValueOrDefault().Date);
    }
}
