namespace ShopOrders.Domain;

/// <summary>
/// Подія, яка намагається змінити стан замовлення.
/// </summary>
public enum OrderEvent
{
    /// <summary>
    /// Оплата замовлення.
    /// </summary>
    Pay,

    /// <summary>
    /// Відвантаження замовлення до доставки.
    /// </summary>
    Ship,

    /// <summary>
    /// Скасування замовлення.
    /// </summary>
    Cancel,
}
