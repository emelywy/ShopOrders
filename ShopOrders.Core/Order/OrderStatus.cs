namespace ShopOrders.Core.Orders;

/// <summary>
/// Статуси життєвого циклу замовлення інтернет-магазину.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Нове замовлення.
    /// </summary>
    New,

    /// <summary>
    /// Замовлення оплачене.
    /// </summary>
    Paid,

    /// <summary>
    /// Замовлення відправлене.
    /// </summary>
    Shipped,

    /// <summary>
    /// Замовлення скасоване.
    /// </summary>
    Cancelled,

    /// <summary>
    /// Замовлення повернене покупцем.
    /// </summary>
    Returned,
}
