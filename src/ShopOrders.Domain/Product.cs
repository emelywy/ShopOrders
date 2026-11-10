namespace ShopOrders.Domain;

/// <summary>
/// Товар каталогу магазину.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Створює товар каталогу.
    /// </summary>
    /// <param name="sku">Артикул товару; не порожній.</param>
    /// <param name="title">Назва товару для показу; не порожня.</param>
    /// <param name="price">Ціна за одиницю в гривнях; невід'ємна.</param>
    /// <exception cref="ArgumentException">Порушено контракт конструктора.</exception>
    public Product(string sku, string title, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfNegative(price);
        Sku = sku;
        Title = title;
        Price = price;
    }

    /// <summary>
    /// Артикул товару, наприклад <c>SKU-104</c>.
    /// </summary>
    public string Sku { get; }

    /// <summary>
    /// Назва товару.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Ціна за одиницю, грн.
    /// </summary>
    public decimal Price { get; }
}
