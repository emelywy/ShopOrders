using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;
/// <summary>Замовлення покупця</summary>
public sealed class Order
{
    private readonly List<OrderLine> _lines = new();
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public DateTimeOffset CreatedAt { get; init; }

    public DeliveryDetails? Delivery { get; set; }
    public PaymentInfo? Payment { get; set; }

    public IReadOnlyList<OrderLine> Lines => _lines;

    public void AddLine(OrderLine line) => _lines.Add(line);

    public decimal Total()
    {
        decimal sum = 0m;
        foreach (OrderLine line in _lines)
        {
            sum += line.Amount;
        }

        if (Delivery != null)
        {
            sum += Delivery.DeliveryCost;
        }

        return sum;
    }
}
