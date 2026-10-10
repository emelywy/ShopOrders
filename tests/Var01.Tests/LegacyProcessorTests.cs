namespace Var01.Tests;

using ShopOrders.Core.Legacy;
using Xunit;

/// <summary>
/// Характеризаційні тести для перевірки поведінки LegacyProcessor.
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
    /// Перевірка повернення -1 при порожньому списку позицій.
    /// </summary>
    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3,
            "Коваль",
            "k@ex.com",
            false,
            new List<OrderLine>(),
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string next);

        Assert.Equal(-1m, total);
    }
}
