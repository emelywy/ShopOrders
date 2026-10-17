using System;
using System.IO;

namespace ShopOrders.Core.Reporting;

/// <summary>
/// Команда для виведення звіту замовлення у потік виводу.
/// </summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output; // 1. Додано підкреслення

    /// <summary>
    /// Ініціалізує новий екземпляр принтера звіту.
    /// </summary>
    /// <param name="output">Цільовий потік запису.</param>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output; // 2. Прибрано this.
    }

    /// <summary>
    /// Виводить сформований звіт у цільовий потік.
    /// </summary>
    /// <param name="report">Рядок сформованого звіту.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report); // 3. Прибрано this.
    }
}
