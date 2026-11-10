namespace ShopOrders.Infrastructure.Records;

/// <summary>
/// Запис позиції замовлення всередині запису <see cref="OrderRecord"/>.
/// </summary>
public sealed class OrderLineRecord
{
    /// <summary>
    /// Артикул товару.
    /// </summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>
    /// Кількість одиниць.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Ціна за одиницю на момент оформлення, грн.
    /// </summary>
    public decimal UnitPrice { get; init; }
}
