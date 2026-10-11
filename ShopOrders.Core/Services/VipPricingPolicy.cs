namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;

/// <summary>
/// Політика ціноутворення для VIP-клієнтів (знижка 10%).
/// </summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * 0.90m, 2);
    }
}
