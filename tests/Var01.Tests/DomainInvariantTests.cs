namespace Var01.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Services;
using Xunit;

/// <summary>
/// Модульні тести для перевірки захисту інваріантів доменної моделі та розрахунку замовлень.
/// </summary>
public class DomainInvariantTests
{
    /// <summary>
    /// Перевіряє, що створення позиції замовлення з нульовою кількістю товару викидає ArgumentOutOfRangeException.
    /// </summary>
    [Fact]
    public void OrderLine_WithZeroQuantity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new OrderLine("SKU-1", 0, 10m));
    }

    /// <summary>
    /// Перевіряє, що додавання нової позиції після переведення замовлення у стан Paid заборонено.
    /// </summary>
    [Fact]
    public void Order_AfterPay_RejectsNewLines()
    {
        Order order = NewOrder(1);
        order.AddLine(new OrderLine("SKU-1", 1, 10m));
        order.Pay();

        Assert.Throws<InvalidOperationException>(() => order.AddLine(new OrderLine("SKU-2", 1, 5m)));
    }

    /// <summary>
    /// Перевіряє правильність розрахунку вартості замовлення із застосуванням 10% знижки через тестову політику.
    /// </summary>
    [Fact]
    public void TotalOf_WithTenPercentDiscount_ReturnsCorrectDiscountedAmount()
    {
        IOrderRepository repository = new InMemoryOrderRepository();
        IPricingPolicy pricing = new DiscountPricingPolicy(0.10m);
        INotifier notifier = new ConsoleNotifier();
        OrderService service = new OrderService(
            repository,
            pricing,
            notifier,
            NullLogger<OrderService>.Instance);

        Order order = NewOrder(2);
        order.AddLine(new OrderLine("SKU-1", 2, 100m));

        decimal expected = 180m; // 2 * 100 = 200; 200 - 10% = 180
        decimal actual = service.TotalOf(order);

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Допоміжний фабричний метод для створення тестового замовлення.
    /// </summary>
    /// <param name="id">Ідентифікатор замовлення.</param>
    /// <returns>Новий екземпляр замовлення зі статусом New.</returns>
    private static Order NewOrder(int id)
    {
        return new Order(id, 100, DateOnly.FromDateTime(DateTime.Today));
    }
}
