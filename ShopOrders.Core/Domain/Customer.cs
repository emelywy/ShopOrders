using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOrders.Core.Domain;
/// <summary>Покупець інтернет-магазину</summary>
public sealed class Customer
{
    /// <summary>Ідентифікатор покупця.</summary>
    public int Id { get; init; }
    /// <summary>Ім'я покупця.</summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>Чи є покупцем постійним клієнтом.</summary>
    public bool IsRegular { get; init; }
}
