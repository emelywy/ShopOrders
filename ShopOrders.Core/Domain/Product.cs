using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Товар у каталозі магазину.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Отримує або задає унікальний артикул товару.
    /// </summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>
    /// Отримує або задає назву товару.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Отримує або задає базову ціну товару, грн.
    /// </summary>
    public decimal Price { get; init; }

    /// <summary>
    /// Отримує або задає кількість товару на складах.
    /// </summary>
    public int Stock { get; init; }
}
