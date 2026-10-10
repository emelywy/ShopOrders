namespace ShopOrders.Core.Orders;

using ShopOrders.Core.Legacy;

/// <summary>
/// Допоміжний клас із захисними умовами для перевірки контрактів замовлення.
/// </summary>
public static class OrderGuards
{
    /// <summary>
    /// Перевіряє коректність основних заголовних даних замовлення.
    /// </summary>
    /// <param name="orderId">Ідентифікатор замовлення.</param>
    /// <param name="clientName">Ім'я клієнта.</param>
    /// <param name="clientEmail">Електронна пошта клієнта.</param>
    public static void EnsureHeaderValid(
        int orderId,
        string clientName,
        string clientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(orderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientEmail);

        if (!clientEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(clientEmail));
        }
    }

    /// <summary>
    /// Перевіряє коректність окремої позиції товару в замовленні.
    /// </summary>
    /// <param name="line">Позиція товару.</param>
    public static void EnsureLineValid(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.Sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            line.Quantity,
            nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(
            line.UnitPrice,
            nameof(line.UnitPrice));
    }

    /// <summary>
    /// Перевіряє коректність усього списку позицій замовлення.
    /// </summary>
    /// <param name="lines">Колекція позицій замовлення.</param>
    public static void EnsureLinesValid(IReadOnlyList<OrderLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Замовлення не містить жодної позиції.",
                nameof(lines));
        }

        foreach (OrderLine line in lines)
        {
            EnsureLineValid(line);
        }
    }
}
