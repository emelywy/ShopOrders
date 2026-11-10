namespace ShopOrders.Domain;

/// <summary>
/// Стан замовлення в його життєвому циклі (таблиця 1.5 специфікації).
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Замовлення оформлене, але ще не оплачене; позиції можна змінювати.
    /// </summary>
    New,

    /// <summary>
    /// Замовлення оплачене й очікує відвантаження.
    /// </summary>
    Paid,

    /// <summary>
    /// Замовлення передане до доставки; стан остаточний.
    /// </summary>
    Shipped,

    /// <summary>
    /// Замовлення анульоване; стан остаточний.
    /// </summary>
    Cancelled,
}
