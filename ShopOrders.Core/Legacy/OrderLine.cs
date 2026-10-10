namespace ShopOrders.Core.Legacy;

/// <summary>
/// Позиція товару для застарілого монолітного обробника.
/// </summary>
public sealed class OrderLine
{
    /// <summary>
    /// Артикул товару.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Кількість одиниць товару.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Ціна за одиницю товару.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
