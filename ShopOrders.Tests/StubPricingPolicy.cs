using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;

namespace ShopOrders.Tests;

/// <summary>Стаб.</summary>
internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    /// <summary>Ініціалізація стаб.</summary>
    /// <param name="price">Ціна.</param>
    public StubPricingPolicy(decimal price) => _price = price;

    /// <summary> Вертаэ цыну.</summary>
    /// <param name="line">Стрічка .</param>
    /// <returns>Вказана ціна.</returns>
    public decimal PriceOf(OrderLine line) => _price;
}
