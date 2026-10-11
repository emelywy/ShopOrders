namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;

/// <summary>
/// Політика ціноутворення з фіксованою відсотковою знижкою.
/// </summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _discountRate;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="DiscountPricingPolicy"/>.
    /// </summary>
    /// <param name="discountRate">Коефіцієнт знижки (від 0.0 до 1.0).</param>
    public DiscountPricingPolicy(decimal discountRate)
    {
        if (discountRate < 0m || discountRate > 1m)
        {
            throw new ArgumentOutOfRangeException(nameof(discountRate), "Знижка повинна бути в діапазоні від 0 до 1.");
        }

        _discountRate = discountRate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return line.Amount * (1m - _discountRate);
    }
}
