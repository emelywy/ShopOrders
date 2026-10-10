namespace ShopOrders.Core.Orders;

using ShopOrders.Core.Legacy;

/// <summary>
/// Калькулятор розрахунку вартості замовлення та метод-диспетчер обробки.
/// </summary>
public static class OrderTotalCalculator
{
    /// <summary>
    /// Метод-диспетчер обробки замовлення, що координує виклики функціональних підпрограм.
    /// </summary>
    /// <param name="orderId">Номер документа замовлення.</param>
    /// <param name="clientName">Ім'я клієнта.</param>
    /// <param name="clientEmail">Електронна пошта клієнта.</param>
    /// <param name="isRegular">Чи є клієнт постійним покупцем.</param>
    /// <param name="lines">Список позицій товарів.</param>
    /// <param name="createdAt">Дата створення замовлення.</param>
    /// <param name="couponCode">Промокод / купон на знижку.</param>
    /// <param name="status">Поточний стан замовлення.</param>
    /// <param name="printToConsole">Прапорець друку чека в консоль.</param>
    /// <param name="deliveryPrice">Базова вартість доставки.</param>
    /// <param name="newStatus">Новий стан замовлення після обробки.</param>
    /// <returns>Підсумкова вартість замовлення.</returns>
    public static decimal Process(
        int orderId,
        string clientName,
        string clientEmail,
        bool isRegular,
        IReadOnlyList<OrderLine> lines,
        DateOnly createdAt,
        string couponCode,
        OrderStatus status,
        bool printToConsole,
        decimal deliveryPrice,
        out OrderStatus newStatus)
    {
        OrderGuards.EnsureHeaderValid(orderId, clientName, clientEmail);

        decimal total = SubtotalOf(lines);
        total -= ClientDiscountOf(total, isRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, couponCode);
        total -= WeekendDiscountOf(total, createdAt);
        total += DeliveryFeeOf(total, deliveryPrice);

        total = status == OrderStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);

        newStatus = NextStatusOf(status, total);

        if (printToConsole)
        {
            Console.WriteLine("Документ No " + orderId);
            Console.WriteLine("Клієнт: " + clientName);
            foreach (OrderLine line in lines)
            {
                Console.WriteLine(line.Sku + " x " + line.Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }

    /// <summary>
    /// Розраховує проміжний підсумок вартості товарів у замовленні.
    /// </summary>
    /// <param name="lines">Список позицій товарів.</param>
    /// <returns>Сума вартості позицій з урахуванням гуртових знижок.</returns>
    public static decimal SubtotalOf(IReadOnlyList<OrderLine> lines)
    {
        OrderGuards.EnsureLinesValid(lines);

        decimal subtotal = 0m;
        foreach (OrderLine line in lines)
        {
            decimal sum = line.Quantity * line.UnitPrice;
            if (line.Quantity >= PricingRules.BulkQuantity)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>
    /// Розраховує розмір знижки постійного покупця.
    /// </summary>
    /// <param name="amount">Поточна сума замовлення.</param>
    /// <param name="isRegularClient">Ознака постійного клієнта.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        return isRegularClient
            ? amount * PricingRules.RegularClientRate
            : 0m;
    }

    /// <summary>
    /// Розраховує порогову знижку від обсягу замовлення.
    /// </summary>
    /// <param name="amount">Поточна сума замовлення.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        if (amount > PricingRules.LargeOrderFrom)
        {
            return amount * PricingRules.LargeOrderRate;
        }

        if (amount > PricingRules.MediumOrderFrom)
        {
            return amount * PricingRules.MediumOrderRate;
        }

        return 0m;
    }

    /// <summary>
    /// Розраховує знижку за купоном або промокодом.
    /// </summary>
    /// <param name="amount">Поточна сума замовлення.</param>
    /// <param name="couponCode">Код купона.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        return couponCode switch
        {
            PricingRules.SaleCoupon
                => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>
    /// Розраховує знижку вихідного дня (для неділі).
    /// </summary>
    /// <param name="amount">Поточна сума замовлення.</param>
    /// <param name="createdAt">Дата оформлення замовлення.</param>
    /// <returns>Розмір знижки.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        return createdAt.DayOfWeek == DayOfWeek.Sunday
            ? amount * PricingRules.WeekendRate
            : 0m;
    }

    /// <summary>
    /// Визначає вартість доставки на основі поточної суми замовлення.
    /// </summary>
    /// <param name="amount">Поточна сума замовлення.</param>
    /// <param name="deliveryPrice">Базова вартість доставки.</param>
    /// <returns>Нарахована вартість доставки або 0 грн при безкоштовній доставці.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);

        return amount < PricingRules.FreeDeliveryFrom
            ? deliveryPrice
            : 0m;
    }

    /// <summary>
    /// Визначає наступний статус замовлення без побічних ефектів.
    /// </summary>
    /// <param name="current">Поточний статус.</param>
    /// <param name="total">Розрахована сума.</param>
    /// <returns>Новий статус замовлення.</returns>
    public static OrderStatus NextStatusOf(OrderStatus current, decimal total)
        => current switch
        {
            OrderStatus.New when total > 0m => OrderStatus.Paid,
            OrderStatus.Paid => OrderStatus.Shipped,
            _ => current,
        };
}
