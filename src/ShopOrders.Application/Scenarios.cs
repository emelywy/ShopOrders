namespace ShopOrders.Application;

/// <summary>
/// Ідентифікатори сценаріїв використання зі специфікації; потрапляють до кожного рядка журналу.
/// </summary>
public static class Scenarios
{
    /// <summary>
    /// UC-01. Оформити замовлення.
    /// </summary>
    public const string PlaceOrder = "UC-01";

    /// <summary>
    /// UC-02. Оплатити замовлення.
    /// </summary>
    public const string PayOrder = "UC-02";

    /// <summary>
    /// UC-03. Скасувати замовлення.
    /// </summary>
    public const string CancelOrder = "UC-03";

    /// <summary>
    /// UC-04. Відвантажити замовлення.
    /// </summary>
    public const string ShipOrder = "UC-04";

    /// <summary>
    /// UC-05. Знайти замовлення.
    /// </summary>
    public const string SearchOrders = "UC-05";

    /// <summary>
    /// UC-06. Звіт «Продажі за період».
    /// </summary>
    public const string SalesByPeriod = "UC-06";
}
