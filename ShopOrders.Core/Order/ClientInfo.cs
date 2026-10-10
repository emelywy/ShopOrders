namespace ShopOrders.Core.Orders;

/// <summary>
/// Информация о клиенте заказа.
/// </summary>
/// <param name="Name">Имя клиента.</param>
/// <param name="Email">Электронная почта клиента.</param>
/// <param name="IsRegular">Признак постоянного клиента.</param>
public sealed record ClientInfo(
    string Name,
    string Email,
    bool IsRegular);
