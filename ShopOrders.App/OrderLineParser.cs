namespace ShopOrders.App;

using System.Globalization;
using ShopOrders.Core.Domain;
using ResultType = ShopOrders.Core.Errors.Result<ShopOrders.Core.Domain.OrderLine>;

/// <summary>
/// Барикада вводу: парсить сирі рядкові дані перед передачею в ядро.
/// </summary>
public static class OrderLineParser
{
    /// <summary>
    /// Парсить рядкові значення у типізований об'єкт рядка замовлення.
    /// </summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="qty">Рядок кількості.</param>
    /// <param name="price">Рядок ціни.</param>
    /// <returns>Результат парсингу.</returns>
    public static ResultType Parse(string sku, string qty, string price)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return ResultType.Fail("Артикул порожній.");
        }

        if (!int.TryParse(qty, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) || q <= 0)
        {
            return ResultType.Fail("Кількість — ціле число більше 0.");
        }

        if (!decimal.TryParse(price, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal p) || p < 0)
        {
            return ResultType.Fail("Ціна не може бути від’ємною.");
        }

        return ResultType.Ok(new OrderLine(sku, q, p));
    }
}
