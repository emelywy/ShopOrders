namespace ShopOrders.Core.Services;

using ShopOrders.Core.Abstractions;
using ShopOrders.Core.Domain;
using ShopOrders.Core.Errors;
using ShopOrders.Core.Orders;

/// <summary>
/// Прикладний сервіс для оформлення та розрахунку вартості замовлень.
/// </summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="OrderService"/>.
    /// </summary>
    /// <param name="repository">Сховище замовлень.</param>
    /// <param name="pricing">Політика ціноутворення.</param>
    /// <param name="notifier">Сервіс сповіщень.</param>
    public OrderService(IOrderRepository repository, IPricingPolicy pricing, INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>
    /// замість важкого try/catch застосовуємо int.TryParse.
    /// </summary>
    /// <param name="rows">Масив рядків зі значеннями кількості.</param>
    /// <returns>Кількість валідних позицій.</returns>
    public static int CountValidQuantities(string[] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, out int q) && q > 0)
            {
                valid++;
            }
        }

        return valid;
    }

    /// <summary>
    /// Зберігає замовлення та сповіщає клієнта.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    /// <param name="customerEmail">Електронна пошта для сповіщення.</param>
    public void Place(Order order, string customerEmail)
    {
        ArgumentNullException.ThrowIfNull(order);

        _repository.Add(order);
        decimal total = TotalOf(order);
        _notifier.Notify(customerEmail, $"Замовлення #{order.Id} оформлено на суму: {total:0.00}");
    }

    /// <summary>
    /// Розраховує загальну вартість замовлення за поточною політикою.
    /// </summary>
    /// <param name="order">Екземпляр замовлення.</param>
    /// <returns>Загальна вартість.</returns>
    public decimal TotalOf(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        decimal total = 0m;
        foreach (OrderLine line in order.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }

    /// <summary>
    /// Додає новий рядок до існуючого замовлення.
    /// </summary>
    /// <param name="orderId">Ідентифікатор замовлення.</param>
    /// <param name="line">Типізований рядок замовлення.</param>
    public void AddLine(int orderId, OrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        Order order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено.");

        if (order.Status != OrderStatus.New)
        {
            throw new DomainRuleException("order.editable", "Редагувати можна лише нове замовлення.");
        }

        order.AddLine(line);
    }

    /// <summary>
    /// Проводить оплату замовлення.
    /// </summary>
    /// <param name="orderId">Ідентифікатор замовлення.</param>
    /// <param name="amount">Сума оплати.</param>
    public void Pay(int orderId, decimal amount)
    {
        Order order = _repository.GetById(orderId)
            ?? throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено.");

        if (order.Status != OrderStatus.New)
        {
            throw new DomainRuleException("order.payable", $"Стан {order.Status} не дозволяє оплату.");
        }

        decimal requiredTotal = TotalOf(order);
        if (amount < requiredTotal)
        {
            throw new DomainRuleException("order.amount", $"Сплачено {amount:0.00}, потрібно {requiredTotal:0.00}.");
        }

        order.Pay();
    }

    /// <summary>
    ///  репозиторій повертає значення штатно,
    /// ми не ковтаємо помилки сховища, а дозволяємо їм підніматися угору.
    /// </summary>
    /// <param name="orderId">Ідентифікатор замовлення.</param>
    /// <returns>Знайдене замовлення або null.</returns>
    public Order? Load(int orderId)
    {
        return _repository.GetById(orderId);
    }

    /// <summary>
    ///  перехоплюємо конкретний тип збою сховища (IOException),
    /// а замість 'throw ex' використовуємо 'throw;' для збереження первинного стека.
    /// </summary>
    /// <param name="orderId">Ідентифікатор замовлення.</param>
    /// <param name="reason">Причина скасування замовлення.</param>
    public void Cancel(int orderId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        try
        {
            Order order = _repository.GetById(orderId)
                ?? throw new DomainRuleException("order.exists", $"Замовлення {orderId} не знайдено.");

            order.Cancel();
        }
        catch (IOException)
        {
            throw;
        }
    }
}
