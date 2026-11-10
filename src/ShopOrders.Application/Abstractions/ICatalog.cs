namespace ShopOrders.Application.Abstractions;

using ShopOrders.Domain;

/// <summary>
/// Довідники товарів і покупців, потрібні для оформлення замовлення.
/// </summary>
public interface ICatalog
{
    /// <summary>
    /// Шукає товар за артикулом без урахування регістру.
    /// </summary>
    /// <param name="sku">Артикул товару.</param>
    /// <returns>Товар або <see langword="null"/>, якщо його немає в каталозі.</returns>
    Product? FindProduct(string sku);

    /// <summary>
    /// Шукає покупця за номером.
    /// </summary>
    /// <param name="id">Номер покупця.</param>
    /// <returns>Покупець або <see langword="null"/>, якщо його немає.</returns>
    Customer? FindCustomer(int id);
}
