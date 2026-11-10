namespace ShopOrders.App;

using ShopOrders.Application.Abstractions;
using ShopOrders.Domain;

/// <summary>
/// Друк складу замовлення на екран.
/// </summary>
public static class OrderPrinter
{
    /// <summary>
    /// Виводить шапку, позиції й підсумки замовлення.
    /// </summary>
    /// <param name="ui">Інтерфейс виводу.</param>
    /// <param name="order">Замовлення для друку.</param>
    /// <param name="catalog">Довідники для назв товарів та імені покупця.</param>
    public static void Print(ConsoleUi ui, Order order, ICatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(ui);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(catalog);
        string customer = catalog.FindCustomer(order.CustomerId)?.FullName ?? "невідомий покупець";
        string header = $"Замовлення №{order.Id} від {Display.Date(order.CreatedAt)}, {customer}, "
            + $"стан: {Display.Status(order.Status)}";
        ui.WriteLine(header);
        foreach (OrderLine line in order.Lines)
        {
            string title = catalog.FindProduct(line.Sku)?.Title ?? line.Sku;
            string price = $"{Display.Money(line.UnitPrice)} = {Display.Money(line.Amount)} грн";
            ui.WriteLine($"  {line.Sku} {title}: {line.Quantity} шт. × {price}");
        }

        string totals = $"Сума: {Display.Money(order.Subtotal)} грн; "
            + $"знижка: {Display.Money(order.Discount)} грн; "
            + $"до сплати: {Display.Money(order.Total)} грн";
        ui.WriteLine(totals);
    }
}
