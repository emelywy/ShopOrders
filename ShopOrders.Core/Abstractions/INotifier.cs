namespace ShopOrders.Core.Abstractions;

/// <summary>
/// Вузький інтерфейс для надсилання сповіщень.
/// </summary>
public interface INotifier
{
    /// <summary>
    /// Надсилає повідомлення адресату.
    /// </summary>
    /// <param name="recipient">Адресат сповіщення.</param>
    /// <param name="message">Текст повідомлення.</param>
    void Notify(string recipient, string message);
}
