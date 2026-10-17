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
}
