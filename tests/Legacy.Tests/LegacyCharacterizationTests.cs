using System.Globalization;
using Variant.Legacy;
using Xunit;

namespace Variant1.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // Фіксуємо культуру, щоб коми/крапки у числах не залежали від мови Windows
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<OrderLineItem> TwoLines() => new()
    {
        new OrderLineItem { Code = "A-1", Qty = 2, Price = 150m },
        new OrderLineItem { Code = "B-2", Qty = 1, Price = 400m }
    };

    // 1. Покриття всіх 4 помилкових гілок
    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, string state, string mail, string expected)
    {
        var items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<OrderLineItem>(),
            _ => null
        };

        var sut = new LegacyOrderProcessor();
        var actual = sut.Handle(1001, 7, "Іван", mail, "regular", 0, items, state, "UAH", new DateTime(2026, 3, 1), false);
        Assert.Equal(expected, actual);
    }

    // 2. Golden Master тест для повного тексту звіту
    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyOrderProcessor();
        var report = sut.Handle(1001, 7, "Іван", "a@b.c", "vip", 3, TwoLines(), "new", "UAH", new DateTime(2026, 3, 1), false);

        var expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "A-1 x2 = 300.00 UAH\n" +
            "B-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";

        Assert.Equal(expected, report);
    }

    // 3. Покриття трьох знижок та варіанта без знижки
    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(string kind, int done, decimal expected)
    {
        var sut = new LegacyOrderProcessor();
        var total = sut.Preview(kind, done, TwoLines());
        Assert.Equal(expected, total);
    }

    // 4. Покриття межі безкоштовної доставки (сума рівно 1000m -> доставка 0)
    [Fact]
    public void Preview_SumExactly1000_FreeShipping()
    {
        var items = new List<OrderLineItem>
        {
            new OrderLineItem { Code = "EXACT-1000", Qty = 1, Price = 1000m }
        };

        var sut = new LegacyOrderProcessor();
        var total = sut.Preview("regular", 0, items);
        Assert.Equal(1000m, total);
    }

    // 5. Опис клієнта
    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new ShopCustomer
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = "vip",
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1)
        };

        var years = DateTime.Now.Year - 2020;
        var expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;
        var actual = new LegacyOrderProcessor().DescribeClient(c);
        Assert.Equal(expected, actual);
    }

    // 6. Логування успішної обробки та відправки пошти
    [Fact]
    public void Handle_SuccessWithMail_WritesLogs()
    {
        var sut = new LegacyOrderProcessor();
        sut.Handle(1001, 7, "Іван", "a@b.c", "regular", 0, TwoLines(), "new", "UAH", new DateTime(2026, 3, 1), true);

        var log = sut.DumpLog();
        Assert.Contains("ok 1001\n", log);
        Assert.Contains("mail -> a@b.c\n", log);
    }
}
