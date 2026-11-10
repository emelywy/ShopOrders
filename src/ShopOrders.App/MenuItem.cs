namespace ShopOrders.App;

/// <summary>
/// Пункт головного меню: клавіша, назва й дія.
/// </summary>
public sealed class MenuItem
{
    /// <summary>
    /// Створює пункт меню.
    /// </summary>
    /// <param name="key">Клавіша, яку вводить оператор.</param>
    /// <param name="title">Назва пункту з ідентифікатором сценарію.</param>
    /// <param name="run">Дія, що виконує сценарій.</param>
    public MenuItem(string key, string title, Action run)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(run);
        Key = key;
        Title = title;
        Run = run;
    }

    /// <summary>
    /// Клавіша пункту.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Назва пункту.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Дія пункту.
    /// </summary>
    public Action Run { get; }
}
