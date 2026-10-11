namespace ShopOrders.Core.Abstractions;

using ShopOrders.Core.Domain;

/// <summary>
/// Інтерфейс репозиторію для збереження та отримання замовлень.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Додає нове замовлення до сховища.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    void Add(Order order);

    /// <summary>
    /// Отримує замовлення за його ідентифікатором.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор замовлення.</param>
    /// <returns>Знайдене замовлення або null.</returns>
    Order? GetById(int id);

    /// <summary>
    /// Отримує список усіх збережених замовлень.
    /// </summary>
    /// <returns>Колекція замовлень лише для читання.</returns>
    IReadOnlyList<Order> GetAll();
}
