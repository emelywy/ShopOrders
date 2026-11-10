namespace ShopOrders.Tests;

using ShopOrders.Application.Pricing;
using ShopOrders.Domain;

/// <summary>
/// Профільне правило варіанта: таблиця рішень 1.4 специфікації, межі й округлення.
/// </summary>
public sealed class TieredDiscountPolicyTests
{
    private readonly TieredDiscountPolicy _policy = new();

    /// <summary>
    /// Дев'ять рядків таблиці рішень: сума, статус, очікувана знижка.
    /// </summary>
    public static TheoryData<decimal, CustomerStatus, decimal> DecisionTable => new()
    {
        { 500m, CustomerStatus.Standard, 0m },
        { 500m, CustomerStatus.Loyal, 15m },
        { 500m, CustomerStatus.Vip, 25m },
        { 2000m, CustomerStatus.Standard, 40m },
        { 2000m, CustomerStatus.Loyal, 100m },
        { 2000m, CustomerStatus.Vip, 140m },
        { 5500m, CustomerStatus.Standard, 220m },
        { 5500m, CustomerStatus.Loyal, 385m },
        { 5500m, CustomerStatus.Vip, 550m },
    };

    /// <summary>
    /// Межі категорій, округлення половини копійки, нуль і верхня межа знижки.
    /// </summary>
    public static TheoryData<decimal, CustomerStatus, decimal> BoundaryCases => new()
    {
        { 0m, CustomerStatus.Vip, 0m },
        { 999.99m, CustomerStatus.Loyal, 30.00m },
        { 1000m, CustomerStatus.Loyal, 50m },
        { 4999.99m, CustomerStatus.Standard, 100.00m },
        { 5000m, CustomerStatus.Vip, 500m },
        { 333.50m, CustomerStatus.Loyal, 10.01m },
        { 6000m, CustomerStatus.Vip, 600m },
        { 8000m, CustomerStatus.Vip, 600m },
        { 20000m, CustomerStatus.Standard, 600m },
    };

    /// <summary>
    /// Суми на межах цінових категорій.
    /// </summary>
    public static TheoryData<decimal, OrderTier> TierBoundaries => new()
    {
        { 999.99m, OrderTier.Small },
        { 1000m, OrderTier.Medium },
        { 4999.99m, OrderTier.Medium },
        { 5000m, OrderTier.Large },
    };

    [Theory]
    [MemberData(nameof(DecisionTable))]
    public void DiscountFor_DecisionTableRow_ReturnsDiscountOfRow(
        decimal subtotal,
        CustomerStatus status,
        decimal expected)
    {
        decimal actual = _policy.DiscountFor(subtotal, status);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(BoundaryCases))]
    public void DiscountFor_BoundaryOrRoundingCase_ReturnsExpected(
        decimal subtotal,
        CustomerStatus status,
        decimal expected)
    {
        decimal actual = _policy.DiscountFor(subtotal, status);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(TierBoundaries))]
    public void TierOf_SumOnBoundary_ReturnsTier(decimal subtotal, OrderTier expected)
    {
        OrderTier actual = TieredDiscountPolicy.TierOf(subtotal);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DiscountFor_NegativeSubtotal_ThrowsArgumentOutOfRange()
    {
        void Act() => _policy.DiscountFor(-0.01m, CustomerStatus.Standard);

        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }
}
