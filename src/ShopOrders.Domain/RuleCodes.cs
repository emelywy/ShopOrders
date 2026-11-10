namespace ShopOrders.Domain;

/// <summary>
/// Коди правил домену, за якими виняток <see cref="DomainRuleException"/> розпізнають
/// у журналі й тестах.
/// </summary>
public static class RuleCodes
{
    /// <summary>
    /// Подію заборонено таблицею переходів станів.
    /// </summary>
    public const string ForbiddenTransition = "order.transition";

    /// <summary>
    /// Змінювати позиції чи знижку можна лише в стані New.
    /// </summary>
    public const string NotEditable = "order.not-editable";

    /// <summary>
    /// Замовлення не містить жодної позиції.
    /// </summary>
    public const string EmptyOrder = "order.empty";

    /// <summary>
    /// Внесена сума менша за суму до сплати.
    /// </summary>
    public const string InsufficientPayment = "payment.insufficient";

    /// <summary>
    /// Замовлення з таким номером не існує.
    /// </summary>
    public const string OrderNotFound = "order.not-found";

    /// <summary>
    /// Покупця з таким номером не існує.
    /// </summary>
    public const string CustomerNotFound = "customer.not-found";

    /// <summary>
    /// Товару з таким артикулом немає в каталозі.
    /// </summary>
    public const string ProductNotFound = "product.not-found";

    /// <summary>
    /// Код для винятків, створених без зазначення правила.
    /// </summary>
    public const string Unspecified = "unspecified";
}
