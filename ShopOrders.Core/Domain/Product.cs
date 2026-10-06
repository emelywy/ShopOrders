using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;
/// <summary>Товар у каталозі магазину</summary>
public sealed class Product
{
    public string Sku { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
