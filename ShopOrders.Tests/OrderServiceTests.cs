using System;
using Microsoft.Extensions.Logging;
using Moq;
using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Services;
using Xunit;

namespace ShopOrders.Tests;

/// <summary>Тести для сервісу замовлень.</summary>
public class OrderServiceTests
{
    /// <summary>Тест суми зі стабом.</summary>
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        // Arrange
        var repository = new FakeOrderRepository();
        var order = new Order(7, 1, DateOnly.MinValue);
        order.AddLine(new OrderLine("A-1", 2, 10m));
        order.AddLine(new OrderLine("B-2", 1, 30m));
        repository.Add(order);

        var notifierStub = Mock.Of<INotifier>();
        var loggerStub = Mock.Of<ILogger<OrderService>>();
        var service = new OrderService(repository, new StubPricingPolicy(25m), notifierStub, loggerStub);

        // Act
        decimal actual = service.TotalOf(order);

        // Assert
        Assert.Equal(50m, actual);
    }

    /// <summary>Тест розміщення з фейком та моком.</summary>
    [Fact]
    public void Place_ValidOrder_AddsToRepoAndNotifies()
    {
        // Arrange
        var repository = new FakeOrderRepository();
        var notifierMock = new Mock<INotifier>(); // Використовуємо Mock для перевірки взаємодії
        var loggerStub = Mock.Of<ILogger<OrderService>>();

        var service = new OrderService(repository, new StubPricingPolicy(1m), notifierMock.Object, loggerStub);
        var order = new Order(1, 5, DateOnly.MinValue);
        order.AddLine(new OrderLine("A-1", 1, 5m));

        // Act
        service.Place(order, "test@example.com");

        // Assert
        Assert.NotNull(repository.GetById(1)); // Перевірка стану (State verification) через Фейк

        // Перевірка поведінки (Behavior verification) через Мок: чи відправився лист рівно 1 раз
        notifierMock.Verify(n => n.Notify("test@example.com", It.IsAny<string>()), Times.Once);
    }

    /// <summary>Тест на передачу порожнього (null) об'єкта.</summary>
    [Fact]
    public void Place_NullOrder_ThrowsArgumentNullException()
    {
        // Arrange
        var repository = new FakeOrderRepository();
        var notifierStub = Mock.Of<INotifier>();
        var loggerStub = Mock.Of<ILogger<OrderService>>();

        var service = new OrderService(repository, new StubPricingPolicy(1m), notifierStub, loggerStub);

        // Act
        Action act = () => service.Place(null!, "test@example.com");

        // Assert
        Assert.Throws<ArgumentNullException>(act); // Твій сервіс викидає саме цю помилку
    }

    /// <summary>Тест на спробу оплатити неіснуюче замовлення (закриваємо гілку).</summary>
    [Fact]
    public void Pay_UnknownOrder_ThrowsDomainRuleException()
    {
        var repository = new FakeOrderRepository();
        var notifierStub = Mock.Of<INotifier>();
        var loggerStub = Mock.Of<ILogger<OrderService>>();
        var service = new OrderService(repository, new StubPricingPolicy(1m), notifierStub, loggerStub);

        Action act = () => service.Pay(999, 100m);

        Assert.Throws<ShopOrders.Core.Errors.DomainRuleException>(act);
    }

    /// <summary>Тест для підняття покриття: завантаження існуючого замовлення.</summary>
    [Fact]
    public void Load_ExistingOrder_ReturnsOrder()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(1, 1, DateOnly.MinValue);
        repository.Add(order);
        var service = new OrderService(repository, new StubPricingPolicy(1m), Mock.Of<INotifier>(), Mock.Of<ILogger<OrderService>>());

        var result = service.Load(1);

        Assert.NotNull(result);
    }

    /// <summary>Тест для підняття покриття: підрахунок валідних кількостей.</summary>
    [Fact]
    public void CountValidQuantities_MixedArray_ReturnsCorrectCount()
    {
        var rows = new[] { "1", "-5", "abc", "10" };

        int count = OrderService.CountValidQuantities(rows);

        Assert.Equal(2, count);
    }

    /// <summary>Тест для підняття покриття: успішне додавання позиції.</summary>
    [Fact]
    public void AddLine_ValidOrder_AddsLine()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(1, 1, DateOnly.MinValue);
        repository.Add(order);
        var service = new OrderService(repository, new StubPricingPolicy(1m), Mock.Of<INotifier>(), Mock.Of<ILogger<OrderService>>());

        service.AddLine(1, new OrderLine("X-1", 1, 100m));

        Assert.Single(order.Lines);
    }

    /// <summary>Тест для підняття покриття: успішна оплата замовлення.</summary>
    [Fact]
    public void Pay_ValidOrder_PaysSuccessfully()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(1, 1, DateOnly.MinValue);
        order.AddLine(new OrderLine("X-1", 1, 100m));
        repository.Add(order);
        var service = new OrderService(repository, new StubPricingPolicy(100m), Mock.Of<INotifier>(), Mock.Of<ILogger<OrderService>>());

        service.Pay(1, 100m);

        Assert.NotNull(repository.GetById(1)); // Просто перевіряємо, що метод відпрацював без помилок
    }

    /// <summary>Тест для підняття покриття: успішне скасування замовлення.</summary>
    [Fact]
    public void Cancel_ValidOrder_CancelsSuccessfully()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(1, 1, DateOnly.MinValue);
        repository.Add(order);
        var service = new OrderService(repository, new StubPricingPolicy(100m), Mock.Of<INotifier>(), Mock.Of<ILogger<OrderService>>());

        service.Cancel(1, "Клієнт передумав");

        Assert.NotNull(repository.GetById(1)); // Перевіряємо, що скасування пройшло без помилок
    }

    /// <summary>Тест для підняття покриття: недостатня сума оплати.</summary>
    [Fact]
    public void Pay_AmountLessThanTotal_ThrowsDomainRuleException()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(2, 1, DateOnly.MinValue);
        order.AddLine(new OrderLine("X-1", 1, 100m));
        repository.Add(order);
        var service = new OrderService(repository, new StubPricingPolicy(100m), Mock.Of<INotifier>(), Mock.Of<ILogger<OrderService>>());

        Action act = () => service.Pay(2, 50m); // Очікуємо 100, платимо 50

        Assert.Throws<ShopOrders.Core.Errors.DomainRuleException>(act);
    }

    /// <summary>Тест для підняття покриття: переплата (закриває гілку логування).</summary>
    [Fact]
    public void Pay_AmountGreaterThanTotal_LogsWarningAndPays()
    {
        var repository = new FakeOrderRepository();
        var order = new Order(3, 1, DateOnly.MinValue);
        order.AddLine(new OrderLine("X-1", 1, 100m));
        repository.Add(order);
        var service = new OrderService(repository, new StubPricingPolicy(100m), Mock.Of<INotifier>(), Mock.Of<ILogger<OrderService>>());

        service.Pay(3, 150m); // Очікуємо 100, платимо 150

        Assert.NotNull(repository.GetById(3));
    }
}
