namespace ShopOrders.Application;

/// <summary>
/// Позиція, яку оператор хоче додати до нового замовлення.
/// </summary>
public sealed class OrderLineRequest
{
    /// <summary>
    /// Створює запит на позицію з уже перевіреними барикадою значеннями.
    /// </summary>
    /// <param name="sku">Нормалізований артикул товару.</param>
    /// <param name="quantity">Кількість одиниць; додатна.</param>
    /// <exception cref="ArgumentException">Порушено контракт: барикаду оминули.</exception>
    public OrderLineRequest(string sku, int quantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        Sku = sku;
        Quantity = quantity;
    }

    /// <summary>
    /// Артикул товару.
    /// </summary>
    public string Sku { get; }

    /// <summary>
    /// Кількість одиниць.
    /// </summary>
    public int Quantity { get; }
}
