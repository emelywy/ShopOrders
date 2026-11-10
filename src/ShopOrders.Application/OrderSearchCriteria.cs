namespace ShopOrders.Application;

using ShopOrders.Domain;

/// <summary>
/// Критерії пошуку UC-05; незаданий критерій (<see langword="null"/>) не обмежує вибірку.
/// </summary>
public sealed class OrderSearchCriteria
{
    /// <summary>
    /// Номер покупця або <see langword="null"/> для будь-якого покупця.
    /// </summary>
    public int? CustomerId { get; init; }

    /// <summary>
    /// Стан замовлення або <see langword="null"/> для будь-якого стану.
    /// </summary>
    public OrderStatus? Status { get; init; }

    /// <summary>
    /// Найраніша дата створення (включно) або <see langword="null"/>.
    /// </summary>
    public DateOnly? CreatedFrom { get; init; }

    /// <summary>
    /// Найпізніша дата створення (включно) або <see langword="null"/>.
    /// </summary>
    public DateOnly? CreatedTo { get; init; }

    /// <summary>
    /// Перевіряє, чи відповідає замовлення всім заданим критеріям.
    /// </summary>
    /// <param name="order">Замовлення для перевірки.</param>
    /// <returns><see langword="true"/>, якщо всі задані критерії виконано.</returns>
    public bool Matches(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        return MatchesCustomer(order) && MatchesStatus(order) && MatchesPeriod(order);
    }

    private bool MatchesCustomer(Order order)
    {
        return CustomerId is null || order.CustomerId == CustomerId;
    }

    private bool MatchesStatus(Order order)
    {
        return Status is null || order.Status == Status;
    }

    private bool MatchesPeriod(Order order)
    {
        bool afterStart = CreatedFrom is null || order.CreatedAt >= CreatedFrom;
        bool beforeEnd = CreatedTo is null || order.CreatedAt <= CreatedTo;
        return afterStart && beforeEnd;
    }
}
