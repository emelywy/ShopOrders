namespace ShopOrders.Core.Services;

using System.Text;
using ShopOrders.Core.Domain;

/// <summary>
/// Форматувальник звіту замовлень у формат CSV (відповідає лише за текст).
/// </summary>
public static class OrderCsvReport
{
    /// <summary>
    /// Формує CSV рядок для переданого переліку замовлень.
    /// </summary>
    /// <param name="orders">Список замовлень.</param>
    /// <returns>Рядок у форматі CSV.</returns>
    public static string BuildCsv(IEnumerable<Order> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);

        var sb = new StringBuilder();
        sb.AppendLine("id;status;linesCount");

        foreach (var order in orders)
        {
            sb.AppendLine($"{order.Id};{order.Status};{order.Lines.Count}");
        }

        return sb.ToString();
    }
}
