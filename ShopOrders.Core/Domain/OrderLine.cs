namespace ShopOrders.Core.Domain;

/// <summary>
/// Позиція товару в замовленні.
/// </summary>
public class OrderLine
{
    /// <summary>
    /// Артикул товару.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Кількість одиниць.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Ціна за одиницю.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
