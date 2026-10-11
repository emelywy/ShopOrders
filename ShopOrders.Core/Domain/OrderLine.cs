namespace ShopOrders.Core.Domain;

/// <summary>
/// Незмінна позиція замовлення (рядок чека).
/// </summary>
public sealed record OrderLine
{
    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="OrderLine"/>.
    /// </summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="quantity">Кількість одиниць товару.</param>
    /// <param name="unitPrice">Ціна за одиницю товару.</param>
    public OrderLine(string sku, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new ArgumentException("Артикул товару (SKU) не може бути порожнім.", nameof(sku));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Кількість товару повинна бути більшою за 0.");
        }

        if (unitPrice < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Ціна за одиницю не може бути від'ємною.");
        }

        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// Отримує артикул товару.
    /// </summary>
    public string Sku { get; }

    /// <summary>
    /// Отримує кількість одиниць товару.
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Отримує ціну за одиницю товару.
    /// </summary>
    public decimal UnitPrice { get; }

    /// <summary>
    /// Отримує загальну вартість по позиції.
    /// </summary>
    public decimal Amount => UnitPrice * Quantity;
}
