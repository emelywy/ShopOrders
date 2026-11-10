namespace ShopOrders.Infrastructure;

using ShopOrders.Domain;
using ShopOrders.Infrastructure.Records;

/// <summary>
/// Перетворення між доменним замовленням і його записом у файлі.
/// </summary>
internal static class OrderRecordMapper
{
    /// <summary>
    /// Створює запис файлу з доменного замовлення.
    /// </summary>
    /// <param name="order">Замовлення для збереження.</param>
    /// <returns>Запис, готовий до серіалізації.</returns>
    internal static OrderRecord ToRecord(Order order)
    {
        return new OrderRecord
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            PaidAt = order.PaidAt,
            Discount = order.Discount,
            Lines = order.Lines.Select(ToLineRecord).ToList(),
        };
    }

    /// <summary>
    /// Відновлює доменне замовлення, повторюючи події за збереженим станом,
    /// тож відновлене замовлення проходить ті самі правила, що й нове.
    /// </summary>
    /// <param name="source">Прочитаний запис файлу.</param>
    /// <returns>Відновлене замовлення.</returns>
    internal static Order ToDomain(OrderRecord source)
    {
        Order order = new(source.Id, source.CustomerId, source.CreatedAt);
        foreach (OrderLineRecord line in source.Lines)
        {
            order.AddLine(new OrderLine(line.Sku, line.Quantity, line.UnitPrice));
        }

        order.ApplyDiscount(source.Discount);
        ReplayStatus(order, source);
        return order;
    }

    private static OrderLineRecord ToLineRecord(OrderLine line)
    {
        return new OrderLineRecord
        {
            Sku = line.Sku,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
        };
    }

    private static void ReplayStatus(Order order, OrderRecord source)
    {
        if (source.PaidAt is DateTimeOffset paidAt)
        {
            order.Pay(order.Total, paidAt);
        }

        if (source.Status == OrderStatus.Shipped)
        {
            order.Ship();
        }
        else if (source.Status == OrderStatus.Cancelled)
        {
            order.Cancel();
        }
    }
}
