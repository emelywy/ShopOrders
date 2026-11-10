namespace ShopOrders.Application.Abstractions;

using ShopOrders.Domain;

/// <summary>
/// Сховище замовлень. Прикладний шар не знає, де й у якому форматі воно зберігає дані.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Видає номер для нового замовлення.
    /// </summary>
    /// <returns>Номер, більший за всі наявні.</returns>
    int NextId();

    /// <summary>
    /// Додає нове замовлення до сховища (без запису на диск).
    /// </summary>
    /// <param name="order">Замовлення з ще не використаним номером.</param>
    void Add(Order order);

    /// <summary>
    /// Шукає замовлення за номером.
    /// </summary>
    /// <param name="id">Номер замовлення.</param>
    /// <returns>Замовлення або <see langword="null"/>, якщо його немає.</returns>
    Order? FindById(int id);

    /// <summary>
    /// Повертає всі замовлення сховища.
    /// </summary>
    /// <returns>Знімок колекції замовлень.</returns>
    IReadOnlyList<Order> ListAll();

    /// <summary>
    /// Зберігає всі зміни у постійне сховище.
    /// </summary>
    void SaveChanges();
}
