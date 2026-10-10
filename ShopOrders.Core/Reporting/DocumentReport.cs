namespace ShopOrders.Core.Reporting;

using System.Globalization;
using System.Text;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Orders;

/// <summary>
/// Форматувальник текстового звіту замовлення.
/// </summary>
public static class DocumentReport
{
    /// <summary>
    /// Формує текстове представлення звіту замовлення як чиста функція.
    /// </summary>
    /// <param name="request">Вхідні параметри замовлення.</param>
    /// <param name="result">Розраховані результати замовлення.</param>
    /// <returns>Відформатований текстовий звіт.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);

        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();

        text.AppendLine($"Документ No {request.DocumentId}");
        text.AppendLine($"Клієнт: {request.Client.Name}");

        foreach (OrderLine line in request.Lines)
        {
            text.AppendLine($"{line.Sku} x {line.Quantity}");
        }

        text.AppendLine(string.Format(culture, "Разом: {0:F2}", result.Total));
        text.AppendLine($"Стан: {result.Status}");

        return text.ToString();
    }
}
