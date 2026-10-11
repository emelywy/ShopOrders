namespace ShopOrders.Core.Domain;

/// <summary>
/// Незмінні дані про клієнта інтернет-магазину.
/// </summary>
public sealed record Customer
{
    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="Customer"/>.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор клієнта.</param>
    /// <param name="fullName">Повне ім'я клієнта.</param>
    /// <param name="email">Електронна пошта клієнта.</param>
    public Customer(int id, string fullName, string email)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Ідентифікатор клієнта повинен бути додатним числом.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Повне ім'я клієнта не може бути порожнім.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new ArgumentException("Некоректний формат електронної пошти.", nameof(email));
        }

        Id = id;
        FullName = fullName;
        Email = email;
    }

    /// <summary>
    /// Отримує унікальний ідентифікатор клієнта.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Отримує повне ім'я клієнта.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Отримує електронну пошту клієнта.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Отримує ознаку постійного покупця.
    /// </summary>
    public bool IsRegular { get; init; }
}
