namespace ShopOrders.Application.Pricing;

using ShopOrders.Domain;

/// <summary>
/// Профільне правило варіанта: знижка за сумою замовлення й статусом покупця
/// (таблиця рішень 1.4 специфікації) з верхньою межею <see cref="MaxDiscount"/>.
/// </summary>
public sealed class TieredDiscountPolicy : IDiscountPolicy
{
    /// <summary>
    /// Нижня межа категорії <see cref="OrderTier.Medium"/>, грн (включно).
    /// </summary>
    public const decimal MediumOrderFrom = 1000m;

    /// <summary>
    /// Нижня межа категорії <see cref="OrderTier.Large"/>, грн (включно).
    /// </summary>
    public const decimal LargeOrderFrom = 5000m;

    /// <summary>
    /// Найбільша можлива знижка, грн.
    /// </summary>
    public const decimal MaxDiscount = 600m;

    private static Dictionary<(OrderTier Tier, CustomerStatus Status), decimal> Rates { get; } =
        BuildRates();

    /// <summary>
    /// Визначає цінову категорію суми.
    /// </summary>
    /// <param name="subtotal">Сума позицій до знижки, грн.</param>
    /// <returns>Категорія, що відповідає рядку таблиці рішень.</returns>
    public static OrderTier TierOf(decimal subtotal)
    {
        if (subtotal >= LargeOrderFrom)
        {
            return OrderTier.Large;
        }

        return subtotal >= MediumOrderFrom ? OrderTier.Medium : OrderTier.Small;
    }

    /// <summary>
    /// Повертає ставку знижки з таблиці рішень.
    /// </summary>
    /// <param name="subtotal">Сума позицій до знижки, грн.</param>
    /// <param name="status">Статус покупця (стовпчик таблиці).</param>
    /// <returns>Ставка як частка одиниці, наприклад 0,05 для 5 %.</returns>
    public static decimal RateFor(decimal subtotal, CustomerStatus status)
    {
        return Rates[(TierOf(subtotal), status)];
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException">Сума від'ємна.</exception>
    public decimal DiscountFor(decimal subtotal, CustomerStatus status)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(subtotal);
        decimal raw = subtotal * RateFor(subtotal, status);
        return Math.Min(Money.Round(raw), MaxDiscount);
    }

    private static Dictionary<(OrderTier Tier, CustomerStatus Status), decimal> BuildRates()
    {
        return new Dictionary<(OrderTier Tier, CustomerStatus Status), decimal>
        {
            [(OrderTier.Small, CustomerStatus.Standard)] = 0.00m,
            [(OrderTier.Small, CustomerStatus.Loyal)] = 0.03m,
            [(OrderTier.Small, CustomerStatus.Vip)] = 0.05m,
            [(OrderTier.Medium, CustomerStatus.Standard)] = 0.02m,
            [(OrderTier.Medium, CustomerStatus.Loyal)] = 0.05m,
            [(OrderTier.Medium, CustomerStatus.Vip)] = 0.07m,
            [(OrderTier.Large, CustomerStatus.Standard)] = 0.04m,
            [(OrderTier.Large, CustomerStatus.Loyal)] = 0.07m,
            [(OrderTier.Large, CustomerStatus.Vip)] = 0.10m,
        };
    }
}
