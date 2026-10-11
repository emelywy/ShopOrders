namespace ShopOrders.Core.Abstractions;

using ShopOrders.Core.Domain;

/// <summary>
/// Інтерфейс політики ціноутворення для розрахунку вартості позиції.
/// </summary>
public interface IPricingPolicy
{
    /// <summary>
    /// Розраховує підсумкову вартість позиції замовлення відповідно до обраної політики.
    /// </summary>
    /// <param name="line">Позиція замовлення.</param>
    /// <returns>Розрахована вартість позиції.</returns>
    decimal PriceOf(OrderLine line);
}
