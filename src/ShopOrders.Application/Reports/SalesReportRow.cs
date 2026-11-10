namespace ShopOrders.Application.Reports;

using ShopOrders.Domain;

/// <summary>
/// Рядок звіту «Продажі за період»: підсумки продажів за один день оплати.
/// </summary>
public sealed class SalesReportRow
{
    /// <summary>
    /// Створює рядок звіту з продажів одного дня.
    /// </summary>
    /// <param name="date">День оплати.</param>
    /// <param name="orders">Оплачені того дня замовлення; щонайменше одне.</param>
    public SalesReportRow(DateOnly date, IReadOnlyCollection<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        Date = date;
        OrderCount = orders.Count;
        Subtotal = orders.Sum(order => order.Subtotal);
        Discount = orders.Sum(order => order.Discount);
        Revenue = orders.Sum(order => order.Total);
    }

    /// <summary>
    /// День оплати.
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// Кількість оплачених замовлень.
    /// </summary>
    public int OrderCount { get; }

    /// <summary>
    /// Сума позицій до знижки, грн.
    /// </summary>
    public decimal Subtotal { get; }

    /// <summary>
    /// Сума наданих знижок, грн.
    /// </summary>
    public decimal Discount { get; }

    /// <summary>
    /// Виручка (сума до сплати), грн.
    /// </summary>
    public decimal Revenue { get; }
}
