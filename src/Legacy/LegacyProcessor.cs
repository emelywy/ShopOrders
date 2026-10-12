#nullable enable

namespace Variant.Legacy;

public static class PricingRules
{
    public const decimal VipRate = 0.15m;
    public const decimal StaffRate = 0.30m;
    public const decimal LoyalRate = 0.05m;
    public const decimal MaxDiscount = 500m;
    public const decimal FreeShipFrom = 1000m;
    public const decimal ShipCost = 60m;
    public const int LoyalOrders = 10;
}

public class ShopCustomer
{
    public int Id;
    public string Name = "";
    public string Email = "";
    public string Kind = "regular";
    public int DoneCount;
    public DateTime SinceUtc;

    public string Describe()
    {
        string s = Name.Trim().ToUpper();
        if (Kind == "vip")
        {
            s += " [VIP]";
        }

        if (DoneCount > PricingRules.LoyalOrders)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + Email.ToLower() + ">";
        int years = DateTime.Now.Year - SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
}

public class OrderLineItem
{
    public string Code = "";
    public int Qty;
    public decimal Price;
    public string Group = "";
}

public class LegacyOrderProcessor
{
    private readonly List<string> _log = new();

    public string Handle(
        int docId,
        int clientId,
        string clientName,
        string? clientMail,
        string clientKind,
        int clientDone,
        List<OrderLineItem>? items,
        string state,
        string currency,
        DateTime createdAt,
        bool sendMail)
    {
        if (items == null)
        {
            return "ERR: null";
        }

        if (items.Count == 0)
        {
            return "ERR: empty";
        }

        if (state != "new" && state != "paid")
        {
            return "ERR: state";
        }

        if (clientMail == null || !clientMail.Contains("@"))
        {
            return "ERR: mail";
        }

        _log.Add("ok " + docId);

        decimal tmpSum = CalculateSubtotal(items);
        decimal tmpDiscount = CalculateDiscount(tmpSum, clientKind, clientDone);
        decimal ship = CalculateShipping(tmpSum - tmpDiscount);
        decimal total = tmpSum - tmpDiscount + ship;

        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < items.Count; i++)
        {
            txt += items[i].Code + " x" + items[i].Qty + " = " +
                (items[i].Qty * items[i].Price).ToString("0.00") + " " + currency + "\n";
        }

        txt += "Знижка: " + tmpDiscount.ToString("0.00") + " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") + " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") + " " + currency + "\n";

        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }

        return txt;
    }

    public decimal Preview(string clientKind, int clientDone, List<OrderLineItem> items)
    {
        decimal s = CalculateSubtotal(items);
        decimal d = CalculateDiscount(s, clientKind, clientDone);
        decimal sh = CalculateShipping(s - d);
        return s - d + sh;
    }

    public string DescribeClient(ShopCustomer c) => c.Describe();

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log)
        {
            r += l + "\n";
        }

        return r;
    }

    private static decimal CalculateSubtotal(List<OrderLineItem> items)
    {
        decimal sum = 0m;
        foreach (var it in items)
        {
            sum += it.Qty * it.Price;
        }

        return sum;
    }

    private static decimal CalculateDiscount(decimal sum, string clientKind, int clientDone)
    {
        decimal discount = 0m;
        if (clientKind == "vip")
        {
            discount = sum * PricingRules.VipRate;
        }
        else if (clientKind == "staff")
        {
            discount = sum * PricingRules.StaffRate;
        }
        else if (clientDone > PricingRules.LoyalOrders)
        {
            discount = sum * PricingRules.LoyalRate;
        }

        if (discount > PricingRules.MaxDiscount)
        {
            discount = PricingRules.MaxDiscount;
        }

        return discount;
    }

    private static decimal CalculateShipping(decimal payable) =>
        payable < PricingRules.FreeShipFrom ? PricingRules.ShipCost : 0m;
}