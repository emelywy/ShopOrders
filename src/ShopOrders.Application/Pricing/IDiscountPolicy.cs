namespace ShopOrders.Application.Pricing;

using ShopOrders.Domain;

/// <summary>
/// Політика знижки: точка розширення для нових видів знижок.
/// </summary>
public interface IDiscountPolicy
{
    /// <summary>
    /// Обчислює знижку для замовлення.
    /// </summary>
    /// <param name="subtotal">Сума позицій до знижки, грн; невід'ємна.</param>
    /// <param name="status">Статус покупця.</param>
    /// <returns>Знижка в гривнях, округлена до копійок.</returns>
    decimal DiscountFor(decimal subtotal, CustomerStatus status);
}
