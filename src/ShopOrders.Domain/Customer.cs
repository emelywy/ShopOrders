namespace ShopOrders.Domain;

/// <summary>
/// Покупець, на якого оформлюється замовлення.
/// </summary>
public sealed class Customer
{
    /// <summary>
    /// Створює покупця з перевіреними даними.
    /// </summary>
    /// <param name="id">Номер покупця; додатне число.</param>
    /// <param name="fullName">Ім'я для показу оператору; не порожнє.</param>
    /// <param name="status">Статус, що визначає ставку знижки.</param>
    /// <exception cref="ArgumentException">Порушено контракт конструктора.</exception>
    public Customer(int id, string fullName, CustomerStatus status)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "Невідомий статус покупця.");
        }

        Id = id;
        FullName = fullName;
        Status = status;
    }

    /// <summary>
    /// Номер покупця.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Ім'я покупця для показу на екрані (до журналу не записується).
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Статус покупця, від якого залежить знижка.
    /// </summary>
    public CustomerStatus Status { get; }
}
