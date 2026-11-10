namespace ShopOrders.Domain;

/// <summary>
/// Позиція замовлення: товар, кількість і ціна, зафіксована на момент оформлення.
/// </summary>
public sealed class OrderLine
{
    /// <summary>
    /// Створює позицію замовлення.
    /// </summary>
    /// <param name="sku">Артикул товару; не порожній.</param>
    /// <param name="quantity">Кількість одиниць; додатна.</param>
    /// <param name="unitPrice">Ціна за одиницю в гривнях; невід'ємна.</param>
    /// <exception cref="ArgumentException">Порушено контракт конструктора.</exception>
    public OrderLine(string sku, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// Артикул товару.
    /// </summary>
    public string Sku { get; }

    /// <summary>
    /// Кількість одиниць товару.
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Ціна за одиницю, грн.
    /// </summary>
    public decimal UnitPrice { get; }

    /// <summary>
    /// Вартість позиції (ціна × кількість), округлена до копійок.
    /// </summary>
    public decimal Amount => Money.Round(UnitPrice * Quantity);
}
