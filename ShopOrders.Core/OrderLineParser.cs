using System.Globalization;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Orders;

namespace ShopOrders.Core;

/// <summary>
/// Межа між «брудними» даними ззовні та сутностями.
/// </summary>
public static class OrderLineParser
{
    public static bool TryParse(string? raw, out OrderLine? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var p = raw.Split(';');
        if (p.Length != 3 || p[0].Trim().Length is 0 or > 20)
        {
            return false;
        }

        if (!int.TryParse(p[1], out var qty) || qty is < 1 or > 1000)
        {
            return false;
        }

        var ci = CultureInfo.InvariantCulture;
        if (!decimal.TryParse(p[2], NumberStyles.Number, ci, out var price) || price < 0m)
        {
            return false;
        }

        line = new OrderLine(p[0].Trim(), qty, price);
        return true;
    }
}
