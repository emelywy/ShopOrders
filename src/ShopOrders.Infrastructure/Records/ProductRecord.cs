namespace ShopOrders.Infrastructure.Records;

/// <summary>
/// Запис товару у файлі products.json.
/// </summary>
public sealed class ProductRecord
{
    /// <summary>
    /// Артикул товару.
    /// </summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>
    /// Назва товару.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Ціна за одиницю, грн.
    /// </summary>
    public decimal Price { get; init; }
}
