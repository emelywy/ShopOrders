namespace ShopOrders.Core.Legacy;

/// <summary>
/// Клас-Бог, що порушує принципи SOLID.
/// </summary>
public class OrderManager
{
    private readonly FileOrderStore _store = new FileOrderStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    /// <summary>
    /// Оформлює замовлення (порушує SRP, OCP, DIP).
    /// </summary>
    public decimal Place(LegacyOrder order, string customerType)
    {
        decimal total = 0m;
        foreach (var line in order.Lines)
        {
            total += line.UnitPrice * line.Quantity;
        }

        if (customerType == "regular")
        {
            total = total * 0.95m;
        }
        else if (customerType == "vip")
        {
            total = total * 0.90m;
        }
        else if (customerType == "staff")
        {
            total = total * 0.70m;
        }

        if (total > 1000m)
        {
            total = total - 50m;
        }

        order.Total = total;
        order.Status = 1;
        _store.SaveToFile(order, "orders.json");
        _mailer.Send(order.Email, "Order " + order.Id, "" + total);
        return total;
    }

    /// <summary>
    /// Генерує CSV-звіт і пише його у файл.
    /// </summary>
    public string BuildCsvReport(List<LegacyOrder> orders)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in orders)
        {
            text += o.Id + ";" + o.Total + ";" + o.Status;
        }

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    /// <summary>
    /// Архівує замовлення (порушує LSP).
    /// </summary>
    public void Archive(LegacyOrder order)
    {
        LegacyOrder archived = new ArchivedOrder(order.Id);
        foreach (var line in order.Lines)
        {
            archived.AddLine(line); // Викине NotSupportedException під час виконання!
        }

        _store.SaveToFile(archived, "archive.json");
    }
}
