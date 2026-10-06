using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;
/// <summary>Стан замовлення</summary>
public enum OrderStatus
{
    New = 0,
    Paid = 1,
    Shipped = 2,
    Cancelled = 3,
    Returned = 4,
}
