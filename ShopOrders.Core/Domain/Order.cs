namespace ShopOrders.Core.Domain;
using ShopOrders.Core.Orders;

/// <summary>
/// Статуси життєвого циклу замовлення інтернет-магазину.
/// </summary>

/// <summary>
/// Агрегат замовлення: керує позиціями та контролює життєвий цикл переходів стану.
/// </summary>
public sealed class Order
{
    private readonly List<OrderLine> _lines = new();

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="Order"/>.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор замовлення.</param>
    /// <param name="customerId">Ідентифікатор замовника.</param>
    /// <param name="createdAt">Дата створення замовлення.</param>
    public Order(int id, int customerId, DateOnly createdAt)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Ідентифікатор замовлення має бути більшим за нуль.");
        }

        if (customerId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(customerId), "Ідентифікатор покупця має бути більшим за нуль.");
        }

        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        Status = OrderStatus.New;
    }

    /// <summary>
    /// Отримує унікальний ідентифікатор замовлення.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Отримує ідентифікатор замовника.
    /// </summary>
    public int CustomerId { get; }

    /// <summary>
    /// Отримує дату створення замовлення.
    /// </summary>
    public DateOnly CreatedAt { get; }

    /// <summary>
    /// Отримує поточний статус замовлення.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Отримує дату та час відправлення замовлення.
    /// </summary>
    public DateTime? ShippedAt { get; private set; }

    /// <summary>
    /// Отримує список позицій замовлення лише для читання.
    /// </summary>
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    /// <summary>
    /// Додає позицію до замовлення.
    /// </summary>
    /// <param name="line">Позиція товару.</param>
    public void AddLine(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (Status != OrderStatus.New)
        {
            throw new InvalidOperationException("Позиції можна додавати лише до нового замовлення зі статусом New.");
        }

        _lines.Add(line);
    }

    /// <summary>
    /// Переводить замовлення у статус оплаченого.
    /// </summary>
    public void Pay()
    {
        if (Status != OrderStatus.New)
        {
            throw new InvalidOperationException($"Неможливо оплатити замовлення у статусі {Status}. Дозволено лише для New.");
        }

        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Неможливо оплатити порожнє замовлення без жодної позиції.");
        }

        Status = OrderStatus.Paid;
    }

    /// <summary>
    /// Переводить замовлення у статус відправленого.
    /// </summary>
    public void Ship()
    {
        if (Status != OrderStatus.Paid)
        {
            throw new InvalidOperationException("Відправити можна лише попередньо оплачене замовлення зі статусом Paid.");
        }

        Status = OrderStatus.Shipped;

        ShippedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Скасовує замовлення.
    /// </summary>
    public void Cancel()
    {
        if (Status != OrderStatus.New && Status != OrderStatus.Paid)
        {
            throw new InvalidOperationException($"Неможливо скасувати замовлення у статусі {Status}.");
        }

        Status = OrderStatus.Cancelled;
    }

    /// <summary>
    /// Оформлює повернення відправленого замовлення.
    /// </summary>
    /// <param name="returnDate">Дата та час повернення.</param>
    public void Return(DateTime returnDate)
    {
        if (Status != OrderStatus.Shipped || ShippedAt == null)
        {
            throw new InvalidOperationException("Повернути можна лише відправлене замовлення зі статусом Shipped.");
        }

        if ((returnDate - ShippedAt.Value).TotalDays > 14)
        {
            throw new InvalidOperationException("Термін повернення товару (14 днів із дати відправлення) вичерпано.");
        }

        Status = OrderStatus.Returned;
    }
}
