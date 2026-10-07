using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Інформація про оплату замовлення.
/// </summary>
public sealed class PaymentInfo
{
    /// <summary>
    /// Отримує або задає метод оплати.
    /// </summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>
    /// Отримує або задає значення, чи оплачено замовлення.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Отримує або задає суму комісії за транзакцію, грн.
    /// </summary>
    public decimal Commission { get; init; }
}
