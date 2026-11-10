namespace ShopOrders.Application.Pricing;

/// <summary>
/// Цінова категорія замовлення за сумою до знижки (рядок таблиці рішень 1.4).
/// </summary>
public enum OrderTier
{
    /// <summary>
    /// Сума до 999,99 грн.
    /// </summary>
    Small,

    /// <summary>
    /// Сума від 1000,00 до 4999,99 грн.
    /// </summary>
    Medium,

    /// <summary>
    /// Сума від 5000,00 грн.
    /// </summary>
    Large,
}
