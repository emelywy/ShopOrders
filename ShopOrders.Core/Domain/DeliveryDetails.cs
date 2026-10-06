using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;
/// <summary>Деталі доставки замовлення</summary>
public sealed class DeliveryDetails
{
    public string City { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public decimal DeliveryCost { get; init; }
}
