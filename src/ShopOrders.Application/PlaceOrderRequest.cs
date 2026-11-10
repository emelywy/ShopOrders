namespace ShopOrders.Application;

/// <summary>
/// Дані сценарію UC-01 «Оформити замовлення».
/// </summary>
public sealed class PlaceOrderRequest
{
    /// <summary>
    /// Створює запит на оформлення замовлення.
    /// </summary>
    /// <param name="customerId">Номер покупця.</param>
    /// <param name="lines">Позиції замовлення в порядку введення.</param>
    public PlaceOrderRequest(int customerId, IReadOnlyList<OrderLineRequest> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        CustomerId = customerId;
        Lines = lines;
    }

    /// <summary>
    /// Номер покупця.
    /// </summary>
    public int CustomerId { get; }

    /// <summary>
    /// Позиції замовлення.
    /// </summary>
    public IReadOnlyList<OrderLineRequest> Lines { get; }
}
