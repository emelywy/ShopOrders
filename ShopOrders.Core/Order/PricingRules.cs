namespace ShopOrders.Core.Orders;

/// <summary>
/// Фіксовані бізнес-правила та константи розрахунку цін і знижок інтернет-магазину.
/// </summary>
public static class PricingRules
{
    /// <summary>
    /// Мінімальна кількість товару для застосування гуртової знижки.
    /// </summary>
    public const int BulkQuantity = 10;

    /// <summary>
    /// Відсоток гуртової знижки на позицію товару (5%).
    /// </summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>
    /// Відсоток знижки для постійного клієнта (7%).
    /// </summary>
    public const decimal RegularClientRate = 0.07m;

    /// <summary>
    /// Поріг суми для отримання великої порогової знижки (5000 грн).
    /// </summary>
    public const decimal LargeOrderFrom = 5000m;

    /// <summary>
    /// Відсоток великої порогової знижки (10%).
    /// </summary>
    public const decimal LargeOrderRate = 0.10m;

    /// <summary>
    /// Поріг суми для отримання середньої порогової знижки (2000 грн).
    /// </summary>
    public const decimal MediumOrderFrom = 2000m;

    /// <summary>
    /// Відсоток середньої порогової знижки (5%).
    /// </summary>
    public const decimal MediumOrderRate = 0.05m;

    /// <summary>
    /// Відсоток знижки вихідного дня (2%).
    /// </summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>
    /// Відсоток знижки за купоном SALE10 (10%).
    /// </summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>
    /// Фіксована сума знижки за купоном MINUS200 (200 грн).
    /// </summary>
    public const decimal CouponFixed = 200m;

    /// <summary>
    /// Мінімальна сума замовлення для застосування фіксованого купона (1000 грн).
    /// </summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>
    /// Поріг вартості замовлення для безкоштовної доставки (1000 грн).
    /// </summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>
    /// Назва відсоткового промокоду.
    /// </summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>
    /// Назва фіксованого промокоду.
    /// </summary>
    public const string FixedCoupon = "MINUS200";
}
