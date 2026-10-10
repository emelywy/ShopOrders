namespace Var01.Tests;

using ShopOrders.Core.Domain;
using ShopOrders.Core.Orders;
using ShopOrders.Core.Reporting;
using Xunit;

/// <summary>
/// Модульні тести для перевірки розрахунку замовлень, захисних умов та звітності.
/// </summary>
public class LegacyProcessorTests
{
    /// <summary>
    /// Перевірка чистого розрахунку для постійного клієнта.
    /// </summary>
    [Fact]
    public void Calculate_RegularClient_Returns1292()
    {
        TotalRequest request = BuildSampleRequest();

        TotalResult result = OrderTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(OrderStatus.Paid, result.Status);
    }

    /// <summary>
    /// Перевірка нарахування платної доставки для невеликого замовлення.
    /// </summary>
    [Fact]
    public void Calculate_SmallOrder_AddsDelivery()
    {
        TotalRequest request = new(
            DocumentId: 2,
            Client: new ClientInfo("Петренко", "p@ex.com", false),
            Lines: new List<OrderLine>
            {
                new() { Sku = "A1", Quantity = 2, UnitPrice = 200m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: "SALE10",
            Status: OrderStatus.Paid,
            DeliveryPrice: 60m);

        TotalResult result = OrderTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(OrderStatus.Shipped, result.Status);
    }

    /// <summary>
    /// Перевірка того, що порожній список позицій кидає виняток ArgumentException.
    /// </summary>
    [Fact]
    public void Calculate_NoLines_ThrowsArgument()
    {
        List<OrderLine> emptyLines = new();
        Assert.Throws<ArgumentException>(() => OrderGuards.EnsureLinesValid(emptyLines));
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

        Assert.Throws<ArgumentOutOfRangeException>(() => OrderGuards.EnsureLineValid(line));
    }

    /// <summary>
    /// Перевірка включення підсумкового рядка суми у текстовий звіт.
    /// </summary>
    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        TotalRequest request = BuildSampleRequest();
        TotalResult result = OrderTotalCalculator.Calculate(request);

        string text = DocumentReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    /// <summary>
    /// Перевірка роботи команди виведення звіту в потік виводу.
    /// </summary>
    [Fact]
    public void Print_WritesReportToOutput()
    {
        StringWriter writer = new();
        DocumentReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }

    /// <summary>
    /// Допоміжний метод генерації тестового запиту на розрахунок замовлення.
    /// </summary>
    /// <returns>Тестовий екземпляр запиту TotalRequest.</returns>
    private static TotalRequest BuildSampleRequest()
    {
        return new TotalRequest(
            DocumentId: 1,
            Client: new ClientInfo("Іваненко", "i@ex.com", true),
            Lines: new List<OrderLine>
            {
                new() { Sku = "A1", Quantity = 12, UnitPrice = 100m },
                new() { Sku = "B2", Quantity = 1, UnitPrice = 250m },
            },
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: OrderStatus.New,
            DeliveryPrice: 60m);
    }
}
