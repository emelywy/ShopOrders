namespace ShopOrders.Core.Errors;

/// <summary>
/// Порушено правило предметної області.
/// </summary>
public class DomainRuleException : Exception
{
    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="DomainRuleException"/>.
    /// </summary>
    /// <param name="rule">Ідентифікатор правила, наприклад "order.payable".</param>
    /// <param name="message">Опис порушення правила українською мовою.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="DomainRuleException"/>.
    /// </summary>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="message">Опис порушення правила українською мовою.</param>
    /// <param name="inner">Внутрішній виняток.</param>
    public DomainRuleException(string rule, string message, Exception inner)
        : base(message, inner)
    {
        Rule = rule;
    }

    /// <summary>
    /// Отримує ідентифікатор правила.
    /// </summary>
    public string Rule { get; }
}
