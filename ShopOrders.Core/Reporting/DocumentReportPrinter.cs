namespace ShopOrders.Core.Reporting;

/// <summary>
/// Команда для виведення звіту замовлення у потік виводу.
/// </summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter output;

    /// <summary>
    /// Ініціалізує новий екземпляр принтера звіту.
    /// </summary>
    /// <param name="output">Цільовий потік запису.</param>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        this.output = output;
    }

    /// <summary>
    /// Виводить сформований звіт у цільовий потік.
    /// </summary>
    /// <param name="report">Рядок сформованого звіту.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        this.output.Write(report);
    }
}
