namespace ShopOrders.Core.Orders;

using ShopOrders.Core.Domain;

/// <summary>
/// Объект параметров запроса на расчет заказа.
/// </summary>
/// <param name="DocumentId">Идентификатор документа/заказа.</param>
/// <param name="Client">Данные клиента.</param>
/// <param name="Lines">Список позиций товаров.</param>
/// <param name="CreatedAt">Дата создания.</param>
/// <param name="CouponCode">Код скидочного купона.</param>
/// <param name="Status">Текущее состояние заказа.</param>
/// <param name="DeliveryPrice">Стоимость доставки.</param>
public sealed record TotalRequest(
    int DocumentId,
    ClientInfo Client,
    IReadOnlyList<OrderLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    OrderStatus Status,
    decimal DeliveryPrice);
