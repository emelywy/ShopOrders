namespace ShopOrders.Domain;

/// <summary>
/// Статус покупця, від якого залежить ставка знижки.
/// </summary>
public enum CustomerStatus
{
    /// <summary>
    /// Звичайний покупець.
    /// </summary>
    Standard,

    /// <summary>
    /// Постійний покупець.
    /// </summary>
    Loyal,

    /// <summary>
    /// VIP-покупець з найбільшою ставкою знижки.
    /// </summary>
    Vip,
}
