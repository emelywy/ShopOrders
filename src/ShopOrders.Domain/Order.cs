namespace ShopOrders.Domain;

/// <summary>
/// Замовлення інтернет-магазину: документ зі станом, позиціями й зафіксованою знижкою.
/// Стан змінюється лише подіями за таблицею <see cref="OrderTransitions"/>.
/// </summary>
public sealed class Order
{
    private readonly List<OrderLine> _lines = new();

    /// <summary>
    /// Створює нове замовлення в стані <see cref="OrderStatus.New"/> без позицій.
    /// </summary>
    /// <param name="id">Номер замовлення; додатне число.</param>
    /// <param name="customerId">Номер покупця; додатне число.</param>
    /// <param name="createdAt">Дата оформлення.</param>
    /// <exception cref="ArgumentOutOfRangeException">Номер не є додатним.</exception>
    public Order(int id, int customerId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);
        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        Status = OrderStatus.New;
    }

    /// <summary>
    /// Номер замовлення.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Номер покупця.
    /// </summary>
    public int CustomerId { get; }

    /// <summary>
    /// Дата оформлення замовлення.
    /// </summary>
    public DateOnly CreatedAt { get; }

    /// <summary>
    /// Поточний стан замовлення.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Момент оплати; <see langword="null"/>, доки замовлення не оплачене.
    /// </summary>
    public DateTimeOffset? PaidAt { get; private set; }

    /// <summary>
    /// Знижка, зафіксована під час оформлення, грн.
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Позиції замовлення лише для читання.
    /// </summary>
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    /// <summary>
    /// Сума позицій до знижки, грн.
    /// </summary>
    public decimal Subtotal => _lines.Sum(line => line.Amount);

    /// <summary>
    /// Сума до сплати: сума позицій мінус знижка, грн.
    /// </summary>
    public decimal Total => Subtotal - Discount;

    /// <summary>
    /// Додає позицію до нового замовлення.
    /// </summary>
    /// <param name="line">Позиція з перевіреними артикулом, кількістю й ціною.</param>
    /// <exception cref="DomainRuleException">Замовлення вже не в стані New.</exception>
    public void AddLine(OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        EnsureEditable();
        _lines.Add(line);
    }

    /// <summary>
    /// Фіксує знижку, обчислену політикою знижки.
    /// </summary>
    /// <param name="discount">Знижка в гривнях; від 0 до суми позицій включно.</param>
    /// <exception cref="DomainRuleException">Замовлення вже не в стані New.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Знижка від'ємна або більша за суму позицій.
    /// </exception>
    public void ApplyDiscount(decimal discount)
    {
        EnsureEditable();
        ArgumentOutOfRangeException.ThrowIfNegative(discount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(discount, Subtotal);
        Discount = discount;
    }

    /// <summary>
    /// Оплачує замовлення: подія <see cref="OrderEvent.Pay"/>.
    /// </summary>
    /// <param name="amount">Внесена сума, грн; не менша за <see cref="Total"/>.</param>
    /// <param name="paidAt">Момент оплати.</param>
    /// <exception cref="DomainRuleException">
    /// Перехід заборонено, замовлення порожнє або суми недостатньо.
    /// </exception>
    public void Pay(decimal amount, DateTimeOffset paidAt)
    {
        OrderStatus target = RequireTransition(OrderEvent.Pay);
        if (_lines.Count == 0)
        {
            throw new DomainRuleException(
                RuleCodes.EmptyOrder,
                $"Замовлення №{Id} не містить жодної позиції.");
        }

        if (amount < Total)
        {
            string message = $"Внесено {amount:0.00} грн, до сплати {Total:0.00} грн, "
                + $"бракує {Total - amount:0.00} грн.";
            throw new DomainRuleException(RuleCodes.InsufficientPayment, message);
        }

        Status = target;
        PaidAt = paidAt;
    }

    /// <summary>
    /// Відвантажує замовлення: подія <see cref="OrderEvent.Ship"/>.
    /// </summary>
    /// <exception cref="DomainRuleException">Перехід заборонено таблицею.</exception>
    public void Ship()
    {
        Status = RequireTransition(OrderEvent.Ship);
    }

    /// <summary>
    /// Скасовує замовлення: подія <see cref="OrderEvent.Cancel"/>.
    /// </summary>
    /// <exception cref="DomainRuleException">Перехід заборонено таблицею.</exception>
    public void Cancel()
    {
        Status = RequireTransition(OrderEvent.Cancel);
    }

    private static string Describe(OrderEvent orderEvent)
    {
        return orderEvent switch
        {
            OrderEvent.Pay => "оплату",
            OrderEvent.Ship => "відвантаження",
            OrderEvent.Cancel => "скасування",
            _ => orderEvent.ToString(),
        };
    }

    private OrderStatus RequireTransition(OrderEvent orderEvent)
    {
        return OrderTransitions.TargetOf(Status, orderEvent)
            ?? throw new DomainRuleException(
                RuleCodes.ForbiddenTransition,
                $"Замовлення №{Id} у стані {Status}: {Describe(orderEvent)} заборонено.");
    }

    private void EnsureEditable()
    {
        if (Status != OrderStatus.New)
        {
            throw new DomainRuleException(
                RuleCodes.NotEditable,
                $"Замовлення №{Id} у стані {Status} вже не можна змінювати.");
        }
    }
}
