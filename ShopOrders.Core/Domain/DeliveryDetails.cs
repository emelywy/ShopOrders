using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Деталі доставки замовлення.
/// </summary>
public sealed class DeliveryDetails
{
    /// <summary>
    /// Отримує або задає місто доставки.
    /// </summary>
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// Отримує або задає адресу доставки.
    /// </summary>
    public string Address { get; init; } = string.Empty;

    /// <summary>
    /// Отримує або задає вартість доставки, грн.
    /// </summary>
    public decimal DeliveryCost { get; init; }
}
