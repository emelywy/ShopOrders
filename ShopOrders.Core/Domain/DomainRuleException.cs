namespace ShopOrders.Core.Domain;

/// <summary>
/// Виняток порушення бізнес-правил предметної області.
/// </summary>
public sealed class DomainRuleException : Exception
{
    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="DomainRuleException"/>.
    /// </summary>
    /// <param name="code">Унікальний код бізнес-помилки.</param>
    /// <param name="message">Опис помилки.</param>
    public DomainRuleException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="DomainRuleException"/>.
    /// </summary>
    /// <param name="code">Унікальний код бізнес-помилки.</param>
    /// <param name="message">Опис помилки.</param>
    /// <param name="innerException">Внутрішній виняток.</param>
    public DomainRuleException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }

    /// <summary>
    /// Отримує унікальний код помилки.
    /// </summary>
    public string Code { get; }
}
