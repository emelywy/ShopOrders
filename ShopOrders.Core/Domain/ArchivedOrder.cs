namespace ShopOrders.Core.Domain;

using ShopOrders.Core.Orders;

/// <summary>
/// Архівне замовлення: містить замовлення всередині (композиція) без можливості модифікації.
/// </summary>
public sealed class ArchivedOrder
{
    private readonly Order _order;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="ArchivedOrder"/>.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    /// <param name="archivedAt">Дата архівації.</param>
    public ArchivedOrder(Order order, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(order);
        _order = order;
        ArchivedAt = archivedAt;
    }

    /// <summary>
    /// Отримує дату перенесення до архіву.
    /// </summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>
    /// Отримує ідентифікатор замовлення.
    /// </summary>
    public int Id => _order.Id;

    /// <summary>
    /// Отримує статус замовлення.
    /// </summary>
    public OrderStatus Status => _order.Status;

    /// <summary>
    /// Отримує список позицій лише для читання. Методу AddLine немає взагалі.
    /// </summary>
    public IReadOnlyList<OrderLine> Lines => _order.Lines;
}
