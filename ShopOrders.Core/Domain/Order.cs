using System;
using System.Collections.Generic;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Замовлення покупця в інтернет-магазині.
/// </summary>
public class Order
{
    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному покупцю.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному покупцю.</summary>
    private const decimal RegularDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на велике замовлення.</summary>
    private const decimal LargeOrderThreshold = 5000m;

    /// <summary>Частка знижки на велике замовлення.</summary>
    private const decimal LargeOrderDiscountRate = 0.15m;

    /// <summary>Кількість рядків, з якої діє гуртова знижка.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума гуртової знижки, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Межа кількості рядків.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Найкоротше ім’я покупця.</summary>
    private const int MinCustomerNameLength = 2;

    /// <summary>Стан: Нове.</summary>
    private const int StatusNew = 0;

    /// <summary>Стан: Оплачено.</summary>
    private const int StatusPaid = 1;

    /// <summary>Стан: Відправлено.</summary>
    private const int StatusShipped = 2;

    /// <summary>Стан: Скасовано.</summary>
    private const int StatusCancelled = 3;

    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Ініціалізує нове замовлення.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор замовлення.</param>
    /// <param name="customerName">ПІБ або назва покупця.</param>
    public Order(string id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Отримує ідентифікатор замовлення.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Отримує ім'я покупця.
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// Отримує поточний статус замовлення.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Отримує дату та час створення замовлення.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає замовлення у списку за його ідентифікатором.
    /// </summary>
    /// <param name="orders">Список замовлень для пошуку.</param>
    /// <param name="orderId">Ідентифікатор шуканого замовлення.</param>
    /// <returns>Знайдене замовлення або null, якщо не знайдено.</returns>
    public static Order? FindById(List<Order> orders, string orderId)
    {
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Id == orderId)
            {
                return orders[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до замовлення рядок із товаром.
    /// </summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="quantity">Кількість одиниць.</param>
    /// <param name="unitPrice">Ціна за одиницю, грн.</param>
    public void AddLine(string sku, int quantity, decimal unitPrice)
    {
        string[] lineData = new string[3];
        lineData[0] = sku;
        lineData[1] = quantity.ToString();
        lineData[2] = unitPrice.ToString();
        _lines.Add(lineData);
    }

    /// <summary>
    /// Розраховує загальну суму замовлення з урахуванням знижок та податків.
    /// </summary>
    /// <param name="isRegularCustomer">Чи є покупець постійним клієнтом.</param>
    /// <returns>Підсумкова сума до сплати.</returns>
    public decimal CalculateTotal(bool isRegularCustomer)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int k = int.Parse(_lines[i][1]);
            decimal c = decimal.Parse(_lines[i][2]);
            total = total + (k * c);
            lineCount = lineCount + 1;
        }

        if (isRegularCustomer && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeOrderThreshold)
        {
            total *= 1m - LargeOrderDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total += total * VatRate;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан замовлення, якщо перехід дозволений правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан замовлення.</param>
    /// <returns>true, якщо перехід виконано; false, якщо він заборонений.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusPaid)
        {
            Status = StatusPaid;
            return true;
        }

        if (Status == StatusPaid && newStatus == StatusShipped)
        {
            Status = StatusShipped;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи заповнені всі обов'язкові поля замовлення і чи є воно валідним.
    /// </summary>
    /// <returns>true, якщо замовлення валідне; інакше false.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && CustomerName != null && CustomerName.Length > MinCustomerNameLength
            && _lines.Count > 0 && _lines.Count < MaxLineCount
            && Status >= StatusNew && Status <= StatusCancelled)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Формує текстовий звіт за всіма позиціями замовлення.
    /// </summary>
    /// <returns>Рядок із роздруком товарів та підсумковою сумою.</returns>
    public string BuildReport()
    {
        string s = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            s = s + "Товар: " + _lines[i][0] + "; кількість: " + _lines[i][1]
                + "; ціна: " + _lines[i][2] + "; сума: "
                + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        s = s + "Разом: " + CalculateTotal(false) + "\n";
        return s;
    }
}
