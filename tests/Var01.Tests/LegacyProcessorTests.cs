namespace Var01.Tests;

using ShopOrders.Core.Legacy;
using ShopOrders.Core.Orders;
using Xunit;

/// <summary>
/// Характеризаційні тести для перевірки поведінки процесора та захисних умов.
/// </summary>
public class LegacyProcessorTests
{
    /// <summary>
    /// Перевірка розрахунку для постійного клієнта.
    /// </summary>
    [Fact]
    public void Process_RegularClient_Returns1292()
    {
        List<OrderLine> lines = new()
        {
            new OrderLine { Sku = "A1", Quantity = 12, UnitPrice = 100m },
            new OrderLine { Sku = "B2", Quantity = 1, UnitPrice = 250m },
        };

        decimal total = LegacyProcessor.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Paid", next);
    }

    /// <summary>
    /// Перевірка додавання вартості доставки для невеликого замовлення.
    /// </summary>
    [Fact]
    public void Process_SmallOrder_AddsDelivery()
    {
        List<OrderLine> lines = new()
        {
            new OrderLine { Sku = "A1", Quantity = 2, UnitPrice = 200m },
        };

        decimal total = LegacyProcessor.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            "Paid",
            false,
            60m,
            out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Shipped", next);
    }

    /// <summary>
    /// Перевірка того, що порожній список позицій кидає виняток ArgumentException.
    /// </summary>
    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            OrderGuards.EnsureLinesValid(
                new List<OrderLine>()));
    }

    /// <summary>
    /// Перевірка того, що нульова кількість товару кидає ArgumentOutOfRangeException.
    /// </summary>
    [Fact]
    public void Calculate_ZeroQuantity_ThrowsRange()
    {
        OrderLine line = new()
        {
            Sku = "A1",
            Quantity = 0,
            UnitPrice = 10m,
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            OrderGuards.EnsureLineValid(line));
    }
}
