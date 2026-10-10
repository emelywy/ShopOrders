namespace ShopOrders.Core.Orders;

/// <summary>
/// Результат выполнения расчета заказа.
/// </summary>
/// <param name="Total">Итоговая сумма к оплате.</param>
/// <param name="Status">Новое состояние заказа.</param>
public sealed record TotalResult(
    decimal Total,
    OrderStatus Status);
