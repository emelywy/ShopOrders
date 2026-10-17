using System;
using ShopOrders.Core.Domain;
using Xunit;

namespace ShopOrders.Tests;

/// <summary>
/// Тести для перевірки логіки розрахунку знижок.
/// </summary>
public class DiscountPolicyTests
{
    /// <summary>
    /// Перевіряє розрахунок знижки для різних класів еквівалентності та межових значень.
    /// </summary>
    /// <param name="amount">Сума замовлення (передається як double).</param>
    /// <param name="isRegular">Чи є клієнт постійним.</param>
    /// <param name="expected">Очікувана знижка (передається як double).</param>
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, true, 20)]
    [InlineData(999.99, false, 0)]
    [InlineData(999.99, true, 50)]
    [InlineData(1000, false, 100)]
    [InlineData(1000, true, 150)]
    [InlineData(5000, false, 150)]
    public void DiscountFor_Boundaries_MatchesTable(double amount, bool isRegular, double expected)
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor((decimal)amount, isRegular);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    /// <summary>
    /// Перевіряє, що від'ємна сума викликає виняток із правильним іменем параметра.
    /// </summary>
    /// <param name="amount">Від'ємна сума замовлення.</param>
    [Theory]
    [InlineData(-0.01)]
    [InlineData(-50.00)]
    [InlineData(-1000)]
    public void DiscountFor_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        Action act = () => policy.DiscountFor((decimal)amount, true);

        // Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(act);
        Assert.Equal("amount", ex.ParamName); // Перевіряємо, що програма свариться саме на параметр amount
    }

    /// <summary>
    /// Перевіряє, що знижка для величезного замовлення обмежується максимальним лімітом.
    /// </summary>
    [Fact]
    public void DiscountFor_HugeOrder_IsCappedAtMax()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(5000m, true);

        // Assert
        Assert.Equal(150m, actual);
    }
}
