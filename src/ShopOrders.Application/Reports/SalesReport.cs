namespace ShopOrders.Application.Reports;

/// <summary>
/// Звіт «Продажі за період» (UC-06): рядки за днями оплати й загальні підсумки.
/// </summary>
public sealed class SalesReport
{
    /// <summary>
    /// Створює звіт.
    /// </summary>
    /// <param name="periodStart">Перший день періоду.</param>
    /// <param name="periodEnd">Останній день періоду.</param>
    /// <param name="rows">Рядки, впорядковані за датою.</param>
    public SalesReport(DateOnly periodStart, DateOnly periodEnd, IReadOnlyList<SalesReportRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        Rows = rows;
    }

    /// <summary>
    /// Перший день періоду (включно).
    /// </summary>
    public DateOnly PeriodStart { get; }

    /// <summary>
    /// Останній день періоду (включно).
    /// </summary>
    public DateOnly PeriodEnd { get; }

    /// <summary>
    /// Рядки звіту за днями оплати.
    /// </summary>
    public IReadOnlyList<SalesReportRow> Rows { get; }

    /// <summary>
    /// Ознака того, що продажів за період немає.
    /// </summary>
    public bool IsEmpty => Rows.Count == 0;

    /// <summary>
    /// Загальна кількість оплачених замовлень.
    /// </summary>
    public int OrderCount => Rows.Sum(row => row.OrderCount);

    /// <summary>
    /// Загальна сума до знижки, грн.
    /// </summary>
    public decimal Subtotal => Rows.Sum(row => row.Subtotal);

    /// <summary>
    /// Загальна сума знижок, грн.
    /// </summary>
    public decimal Discount => Rows.Sum(row => row.Discount);

    /// <summary>
    /// Загальна виручка, грн.
    /// </summary>
    public decimal Revenue => Rows.Sum(row => row.Revenue);
}
