namespace ShopOrders.Domain;

/// <summary>
/// Таблиця переходів станів замовлення, записана в коді один в один зі специфікацією.
/// </summary>
public static class OrderTransitions
{
    /// <summary>
    /// Визначає новий стан замовлення після події.
    /// </summary>
    /// <param name="current">Поточний стан замовлення.</param>
    /// <param name="orderEvent">Подія, що намагається змінити стан.</param>
    /// <returns>Новий стан або <see langword="null"/>, якщо перехід заборонено.</returns>
    public static OrderStatus? TargetOf(OrderStatus current, OrderEvent orderEvent)
    {
        return (current, orderEvent) switch
        {
            (OrderStatus.New, OrderEvent.Pay) => OrderStatus.Paid,
            (OrderStatus.New, OrderEvent.Cancel) => OrderStatus.Cancelled,
            (OrderStatus.Paid, OrderEvent.Ship) => OrderStatus.Shipped,
            (OrderStatus.Paid, OrderEvent.Cancel) => OrderStatus.Cancelled,
            _ => null,
        };
    }

    /// <summary>
    /// Перевіряє, чи дозволяє таблиця переходів подію в поточному стані.
    /// </summary>
    /// <param name="current">Стан, у якому перебуває замовлення.</param>
    /// <param name="orderEvent">Подія, дозвіл на яку перевіряється.</param>
    /// <returns><see langword="true"/>, якщо перехід дозволено.</returns>
    public static bool IsAllowed(OrderStatus current, OrderEvent orderEvent)
    {
        return TargetOf(current, orderEvent).HasValue;
    }
}
