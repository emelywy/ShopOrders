namespace ShopOrders.Core.Orders;

/// <summary>
/// Перелічення допустимих станів замовлення інтернет-магазину.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Нове замовлення, очікує оплати.
    /// </summary>
    New,

    /// <summary>
    /// Замовлення успішно оплачене.
    /// </summary>
    Paid,

    /// <summary>
    /// Замовлення передане кур'єру або поштовій службі.
    /// </summary>
    Shipped,

    /// <summary>
    /// Замовлення скасоване покупцем або магазином.
    /// </summary>
    Cancelled,
}
