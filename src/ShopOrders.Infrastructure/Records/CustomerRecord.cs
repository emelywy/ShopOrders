namespace ShopOrders.Infrastructure.Records;

using ShopOrders.Domain;

/// <summary>
/// Запис покупця у файлі customers.json (умовні дані, не реальні особи).
/// </summary>
public sealed class CustomerRecord
{
    /// <summary>
    /// Номер покупця.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Умовне ім'я покупця.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Статус покупця.
    /// </summary>
    public CustomerStatus Status { get; init; }
}
