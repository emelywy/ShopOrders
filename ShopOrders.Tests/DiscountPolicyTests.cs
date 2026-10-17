using System;
using ShopOrders.Core.Domain;
using Xunit;

namespace ShopOrders.Tests;

/// <summary>
/// Тести для перевірки логіки
/// </summary>
public class DiscountPolicyTests
{
    /// <summary>
    /// Тест 1:Неправильний
    /// </summary>
    [Fact]
    public void DiscountFor_NegativeAmount_Throws()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        Action act = () => policy.DiscountFor(-0.01m, true);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>("amount", act);
    }

    /// <summary>
    /// Тест 2:Нижня межа.
    /// </summary>
    [Fact]
    public void DiscountFor_ZeroAmount_ReturnsZero()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(0m, false);

        // Assert
        Assert.Equal(0.00m, actual);
    }

    /// <summary>
    /// Тест 3: Звичайне замовлення.
    /// </summary>
    [Fact]
    public void DiscountFor_SmallOrderRegularCustomer_ReturnsRegularDiscount()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(400m, true);

        // Assert
        Assert.Equal(20.00m, actual);
    }

    /// <summary>
    /// Тест 4:Нижня межа
    /// </summary>
    [Fact]
    public void DiscountFor_JustBelowBigOrder_ReturnsZero()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(999.99m, false);

        // Assert
        Assert.Equal(0.00m, actual);
    }

    /// <summary>
    /// Тест 5:Верхня межа
    /// </summary>
    [Fact]
    public void DiscountFor_BigOrderNewCustomer_ReturnsBigOrderDiscount()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, false);

        // Assert
        Assert.Equal(100.00m, actual);
    }

    /// <summary>
    /// Тест 6:Верхня межа
    /// </summary>
    [Fact]
    public void DiscountFor_BigOrderRegularCustomer_ReturnsCombinedDiscount()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(1000m, true);

        // Assert
        Assert.Equal(150.00m, actual);
    }

    /// <summary>
    /// Тест: Некорректний вхід для нового клієнта.
    /// </summary>
    [Fact]
    public void DiscountFor_NegativeAmountNewCustomer_Throws()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        Action act = () => policy.DiscountFor(-50m, false);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>("amount", act);
    }

    /// <summary>
    /// Тест: Типове велике замовлення (середина класу еквівалентності).
    /// </summary>
    [Fact]
    public void DiscountFor_TypicalBigOrder_ReturnsBigOrderDiscount()
    {
        // Arrange
        var policy = new DiscountPolicy();

        // Act
        decimal actual = policy.DiscountFor(5000m, false);

        // Assert
        Assert.Equal(500.00m, actual);
    }
}
