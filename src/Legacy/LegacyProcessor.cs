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

        decimal tmpSum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            tmpSum += items[i].Qty * items[i].Price;
        }

        decimal tmpDiscount = 0m;
        if (clientKind == "vip")
        {
            tmpDiscount = tmpSum * PricingRules.VipRate;
            if (tmpDiscount > PricingRules.MaxDiscount)
            {
                tmpDiscount = PricingRules.MaxDiscount;
            }
        }
        else if (clientKind == "staff")
        {
            tmpDiscount = tmpSum * PricingRules.StaffRate;
            if (tmpDiscount > PricingRules.MaxDiscount)
            {
                tmpDiscount = PricingRules.MaxDiscount;
            }
        }
        else if (clientDone > PricingRules.LoyalOrders)
        {
            tmpDiscount = tmpSum * PricingRules.LoyalRate;
            if (tmpDiscount > PricingRules.MaxDiscount)
            {
                tmpDiscount = PricingRules.MaxDiscount;
            }
        }

        decimal ship = 0m;
        if (tmpSum - tmpDiscount < PricingRules.FreeShipFrom)
        {
            ship = PricingRules.ShipCost;
        }

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
        decimal s = 0m;
        foreach (var it in items)
        {
            s += it.Qty * it.Price;
        }

        decimal d = 0m;
        if (clientKind == "vip")
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaxDiscount)
            {
                d = PricingRules.MaxDiscount;
            }
        }
        else if (clientKind == "staff")
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaxDiscount)
            {
                d = PricingRules.MaxDiscount;
            }
        }
        else if (clientDone > PricingRules.LoyalOrders)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaxDiscount)
            {
                d = PricingRules.MaxDiscount;
            }
        }

        decimal sh = 0m;
        if (s - d < PricingRules.FreeShipFrom)
        {
            sh = PricingRules.ShipCost;
        }

        return s - d + sh;
    }

    public string DescribeClient(ShopCustomer c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == "vip")
        {
            s += " [VIP]";
        }

        if (c.DoneCount > PricingRules.LoyalOrders)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log)
        {
            r += l + "\n";
        }

        return r;
    }
}
