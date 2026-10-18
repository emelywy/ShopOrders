using ShopOrders.Core;
using Xunit;

namespace ShopOrders.Tests;

public class OrderLineParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SKU-1;2")]
    [InlineData("SKU-1;0;10.50")]
    [InlineData("SKU-1;-2;10.50")]
    [InlineData("SKU-1;2;-10.50")]
    public void TryParse_InvalidInput_ReturnsFalse(string raw)
    {
        var ok = OrderLineParser.TryParse(raw, out var line);
        Assert.False(ok);
        Assert.Null(line);
    }
}
