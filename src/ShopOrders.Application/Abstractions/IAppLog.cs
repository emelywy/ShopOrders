namespace ShopOrders.Application.Abstractions;

/// <summary>
/// Журнал роботи застосунку з трьома рівнями. Персональні дані до журналу не передаються.
/// </summary>
public interface IAppLog
{
    /// <summary>
    /// Записує початок або завершення сценарію.
    /// </summary>
    /// <param name="scenario">Ідентифікатор сценарію, наприклад UC-01.</param>
    /// <param name="message">Опис події без персональних даних.</param>
    void LogInformation(string scenario, string message);

    /// <summary>
    /// Записує відхилену операцію (порушення правила домену).
    /// </summary>
    /// <param name="scenario">Сценарій, у якому операцію відхилено.</param>
    /// <param name="message">Причина відхилення.</param>
    void LogWarning(string scenario, string message);

    /// <summary>
    /// Записує виняток, через який операцію не виконано.
    /// </summary>
    /// <param name="scenario">Сценарій або місце, де стався збій.</param>
    /// <param name="message">Контекст збою.</param>
    /// <param name="exception">Виняток, що спричинив збій.</param>
    void LogError(string scenario, string message, Exception exception);
}
