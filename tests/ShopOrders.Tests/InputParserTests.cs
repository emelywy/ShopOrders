namespace ShopOrders.Tests;

using ShopOrders.Application.Input;
using ShopOrders.Domain;

/// <summary>
/// Барикада валідації: порожній рядок, null, від'ємні й нечислові значення, формати.
/// </summary>
public sealed class InputParserTests
{
    public static TheoryData<string, decimal> ValidMoney => new()
    {
        { "249,50", 249.50m },
        { "249.50", 249.50m },
        { " 100 ", 100m },
        { "0", 0m },
    };

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("-3")]
    [InlineData("0")]
    [InlineData("abc")]
    [InlineData("2.5")]
    public void ParsePositiveNumber_InvalidText_Fails(string? text)
    {
        ValidationResult result = InputParser.ParsePositiveNumber(text).Result;

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.ErrorMessage);
    }

    [Fact]
    public void ParsePositiveNumber_NumberWithSpaces_ReturnsValue()
    {
        (ValidationResult result, int value) = InputParser.ParsePositiveNumber(" 42 ");

        Assert.True(result.IsValid);
        Assert.Equal(42, value);
    }

    [Fact]
    public void ParseSku_LowercaseWithSpaces_NormalizesToUpper()
    {
        (ValidationResult result, string sku) = InputParser.ParseSku("  sku-104 ");

        Assert.True(result.IsValid);
        Assert.Equal("SKU-104", sku);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    [InlineData("AB CD")]
    [InlineData("SKU-12345678901234567")]
    public void ParseSku_InvalidText_Fails(string? text)
    {
        ValidationResult result = InputParser.ParseSku(text).Result;

        Assert.False(result.IsValid);
    }

    [Theory]
    [MemberData(nameof(ValidMoney))]
    public void ParseMoney_ValidText_ReturnsAmount(string text, decimal expected)
    {
        (ValidationResult result, decimal amount) = InputParser.ParseMoney(text);

        Assert.True(result.IsValid);
        Assert.Equal(expected, amount);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("1,234")]
    [InlineData("abc")]
    [InlineData("1 000")]
    public void ParseMoney_InvalidText_Fails(string text)
    {
        ValidationResult result = InputParser.ParseMoney(text).Result;

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ParseDate_ValidText_ReturnsDate()
    {
        (ValidationResult result, DateOnly date) = InputParser.ParseDate("01.10.2026");

        Assert.True(result.IsValid);
        Assert.Equal(new DateOnly(2026, 10, 1), date);
    }

    [Theory]
    [InlineData("2026-10-01")]
    [InlineData("31.02.2026")]
    public void ParseDate_WrongFormatOrMissingDay_Fails(string text)
    {
        ValidationResult result = InputParser.ParseDate(text).Result;

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ParseStatus_LowercaseName_IgnoresCase()
    {
        (ValidationResult result, OrderStatus status) = InputParser.ParseStatus("paid");

        Assert.True(result.IsValid);
        Assert.Equal(OrderStatus.Paid, status);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("Returned")]
    public void ParseStatus_NumberOrUnknownName_Fails(string text)
    {
        ValidationResult result = InputParser.ParseStatus(text).Result;

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ParseOptional_EmptyText_ReturnsSuccessWithoutValue()
    {
        (ValidationResult result, int? value) =
            InputParser.ParseOptional(string.Empty, InputParser.ParsePositiveNumber);

        Assert.True(result.IsValid);
        Assert.False(value.HasValue);
    }

    [Fact]
    public void ParseOptional_InvalidText_ReturnsFailureWithoutValue()
    {
        (ValidationResult result, int? value) =
            InputParser.ParseOptional("-5", InputParser.ParsePositiveNumber);

        Assert.False(result.IsValid);
        Assert.False(value.HasValue);
    }

    [Fact]
    public void ValidatePeriod_EndBeforeStart_Fails()
    {
        ValidationResult result =
            InputParser.ValidatePeriod(new DateOnly(2026, 10, 31), new DateOnly(2026, 10, 1));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidatePeriod_SameDay_Succeeds()
    {
        DateOnly day = new(2026, 10, 1);

        ValidationResult result = InputParser.ValidatePeriod(day, day);

        Assert.True(result.IsValid);
    }
}
