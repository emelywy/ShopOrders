using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Перелік можливих станів замовлення.
/// </summary>
public enum OrderStatus
{
    /// <summary>Нове замовлення.</summary>
    New = 0,

    /// <summary>Замовлення оплачено.</summary>
    Paid = 1,

    /// <summary>Замовлення відправлено покупцю.</summary>
    Shipped = 2,

    /// <summary>Замовлення скасовано.</summary>
    Cancelled = 3,

    /// <summary>Замовлення повернено.</summary>
    Returned = 4,
}
