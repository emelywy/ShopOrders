namespace ShopOrders.Domain;

/// <summary>
/// Порушення правила предметної області: операція коректна за формою,
/// але заборонена бізнес-правилом (наприклад, оплата скасованого замовлення).
/// </summary>
public class DomainRuleException : Exception
{
    /// <summary>
    /// Створює виняток із кодом правила та поясненням для користувача.
    /// </summary>
    /// <param name="rule">Код порушеного правила з <see cref="RuleCodes"/>.</param>
    /// <param name="message">Пояснення українською мовою, придатне для показу оператору.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>
    /// Створює виняток без коду правила (стандартний конструктор .NET).
    /// </summary>
    public DomainRuleException()
        : base("Порушено правило предметної області.")
    {
        Rule = RuleCodes.Unspecified;
    }

    /// <summary>
    /// Створює виняток із поясненням без коду правила (стандартний конструктор .NET).
    /// </summary>
    /// <param name="message">Пояснення порушення.</param>
    public DomainRuleException(string message)
        : base(message)
    {
        Rule = RuleCodes.Unspecified;
    }

    /// <summary>
    /// Створює виняток із поясненням і причиною (стандартний конструктор .NET).
    /// </summary>
    /// <param name="message">Пояснення порушення.</param>
    /// <param name="innerException">Виняток, що спричинив порушення.</param>
    public DomainRuleException(string message, Exception innerException)
        : base(message, innerException)
    {
        Rule = RuleCodes.Unspecified;
    }

    /// <summary>
    /// Код порушеного правила, наприклад <c>order.transition</c>.
    /// </summary>
    public string Rule { get; }
}
