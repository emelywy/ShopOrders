namespace ShopOrders.Infrastructure.Records;

using ShopOrders.Domain;

/// <summary>
/// Запис замовлення у файлі orders.json. Окремий тип потрібен, щоб домен
/// не залежав від атрибутів і правил серіалізації.
/// </summary>
public sealed class OrderRecord
{
    /// <summary>
    /// Номер замовлення.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Номер покупця.
    /// </summary>
    public int CustomerId { get; init; }

    /// <summary>
    /// Дата оформлення.
    /// </summary>
    public DateOnly CreatedAt { get; init; }

    /// <summary>
    /// Стан замовлення.
    /// </summary>
    public OrderStatus Status { get; init; }

    /// <summary>
    /// Момент оплати або <see langword="null"/>.
    /// </summary>
    public DateTimeOffset? PaidAt { get; init; }

    /// <summary>
    /// Зафіксована знижка, грн.
    /// </summary>
    public decimal Discount { get; init; }

    /// <summary>
    /// Позиції замовлення.
    /// </summary>
    public IReadOnlyList<OrderLineRecord> Lines { get; init; } = Array.Empty<OrderLineRecord>();
}
