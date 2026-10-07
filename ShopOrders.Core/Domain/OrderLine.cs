using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Рядок замовлення (позиція товару).
/// </summary>
public sealed class OrderLine
{
    /// <summary>
    /// Отримує або задає артикул товару.
    /// </summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>
    /// Отримує або задає кількість одиниць товару.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Отримує або задає ціну за одиницю товару, грн.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Отримує загальну суму за цей рядок (кількість помножена на ціну).
    /// </summary>
    public decimal Amount => Quantity * UnitPrice;
}
