namespace ShopOrders.App;

using System.Globalization;
using ShopOrders.Application.Input;
using ShopOrders.Domain;

/// <summary>
/// Форматування значень для показу оператору.
/// </summary>
public static class Display
{
    /// <summary>
    /// Форматує суму з двома знаками після крапки: 1234.50 (так само, як у журналі).
    /// </summary>
    /// <param name="amount">Сума, грн.</param>
    /// <returns>Текст суми без позначення валюти.</returns>
    public static string Money(decimal amount)
    {
        return amount.ToString("0.00", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Форматує дату як дд.мм.рррр.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <returns>Текст дати.</returns>
    public static string Date(DateOnly date)
    {
        return date.ToString(InputParser.DateFormat, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Назва стану замовлення українською.
    /// </summary>
    /// <param name="status">Стан замовлення.</param>
    /// <returns>Назва стану.</returns>
    public static string Status(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.New => "новий",
            OrderStatus.Paid => "оплачений",
            OrderStatus.Shipped => "відвантажений",
            OrderStatus.Cancelled => "скасований",
            _ => status.ToString(),
        };
    }

    /// <summary>
    /// Назва статусу покупця українською.
    /// </summary>
    /// <param name="status">Статус покупця.</param>
    /// <returns>Назва статусу.</returns>
    public static string Status(CustomerStatus status)
    {
        return status switch
        {
            CustomerStatus.Standard => "звичайний",
            CustomerStatus.Loyal => "постійний",
            CustomerStatus.Vip => "VIP",
            _ => status.ToString(),
        };
    }
}
