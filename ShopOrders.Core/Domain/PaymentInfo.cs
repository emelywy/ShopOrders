using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;
/// <summary>Інформація про оплату замовлення</summary>
public sealed class PaymentInfo
{
    public string PaymentMethod { get; init; } = string.Empty;
    public bool IsPaid { get; set; }
    public decimal Commission { get; init; }
}
