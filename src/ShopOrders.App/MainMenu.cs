namespace ShopOrders.App;

using ShopOrders.Application.Abstractions;
using ShopOrders.Domain;

/// <summary>
/// Головне меню: показує пункти, запускає сценарій і перетворює помилки на повідомлення.
/// </summary>
public sealed class MainMenu
{
    /// <summary>
    /// Клавіша виходу із застосунку.
    /// </summary>
    public const string ExitKey = "0";

    private readonly ConsoleUi _ui;
    private readonly IAppLog _log;
    private readonly List<MenuItem> _items;

    /// <summary>
    /// Створює головне меню.
    /// </summary>
    /// <param name="ui">Інтерфейс вводу й виводу.</param>
    /// <param name="orders">Екрани сценаріїв UC-01 — UC-04.</param>
    /// <param name="reports">Екрани сценаріїв UC-05 і UC-06.</param>
    /// <param name="log">Журнал для запису збоїв.</param>
    public MainMenu(ConsoleUi ui, OrderScreens orders, ReportScreens reports, IAppLog log)
    {
        ArgumentNullException.ThrowIfNull(ui);
        ArgumentNullException.ThrowIfNull(orders);
        ArgumentNullException.ThrowIfNull(reports);
        ArgumentNullException.ThrowIfNull(log);
        _ui = ui;
        _log = log;
        _items = new List<MenuItem>
        {
            new("1", "Оформити замовлення (UC-01)", orders.Place),
            new("2", "Оплатити замовлення (UC-02)", orders.Pay),
            new("3", "Скасувати замовлення (UC-03)", orders.Cancel),
            new("4", "Відвантажити замовлення (UC-04)", orders.Ship),
            new("5", "Знайти замовлення (UC-05)", reports.Search),
            new("6", "Звіт «Продажі за період» (UC-06)", reports.SalesByPeriod),
            new("7", "Переглянути замовлення", orders.Show),
        };
    }

    /// <summary>
    /// Виконує цикл меню до виходу або завершення вводу.
    /// </summary>
    /// <returns>Код завершення процесу: 0 — нормальне завершення.</returns>
    public int Run()
    {
        _ui.WriteLine("ShopOrders — облік замовлень інтернет-магазину");
        while (true)
        {
            PrintMenu();
            string? choice = _ui.ReadLine("> ")?.Trim();
            if (choice is null || choice == ExitKey)
            {
                _ui.WriteLine("Роботу завершено.");
                return 0;
            }

            if (!Execute(choice))
            {
                _ui.WriteLine(ConsoleUi.InputClosedMessage);
                return 0;
            }
        }
    }

    private void PrintMenu()
    {
        _ui.WriteLine(string.Empty);
        foreach (MenuItem item in _items)
        {
            _ui.WriteLine($"{item.Key}. {item.Title}");
        }

        _ui.WriteLine($"{ExitKey}. Вихід");
    }

    private bool Execute(string choice)
    {
        MenuItem? item = _items.Find(candidate => candidate.Key == choice);
        if (item is null)
        {
            _ui.WriteLine("Немає такого пункту меню, спробуйте ще раз.");
            return true;
        }

        try
        {
            item.Run();
        }
        catch (DomainRuleException ex)
        {
            _ui.WriteLine($"Операцію відхилено: {ex.Message}");
        }
        catch (IOException ex)
        {
            _log.LogError($"menu-{item.Key}", "Не вдалося прочитати або записати файл даних.", ex);
            _ui.WriteLine($"Помилка файлу даних: {ex.Message} Перевірте теку data і повторіть.");
        }
        catch (OperationCanceledException)
        {
            return false;
        }

        return true;
    }
}
