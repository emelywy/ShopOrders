namespace ShopOrders.Infrastructure;

using ShopOrders.Application.Abstractions;
using ShopOrders.Domain;
using ShopOrders.Infrastructure.Records;

/// <summary>
/// Сховище замовлень у файлі JSON: читається цілком під час запуску,
/// записується цілком під час збереження.
/// </summary>
public sealed class JsonOrderRepository : IOrderRepository
{
    /// <summary>
    /// Ім'я робочого файлу замовлень.
    /// </summary>
    public const string FileName = "orders.json";

    /// <summary>
    /// Ім'я файлу з початковими умовними замовленнями.
    /// </summary>
    public const string SeedFileName = "orders.seed.json";

    private readonly string _filePath;
    private readonly Dictionary<int, Order> _orders;

    private JsonOrderRepository(string filePath, Dictionary<int, Order> orders)
    {
        _filePath = filePath;
        _orders = orders;
    }

    /// <summary>
    /// Відкриває сховище в теці даних; за відсутності робочого файлу копіює початкові дані.
    /// </summary>
    /// <param name="dataDirectory">Тека з файлами даних.</param>
    /// <returns>Сховище із завантаженими замовленнями.</returns>
    /// <exception cref="IOException">Файл недоступний для читання.</exception>
    /// <exception cref="System.Text.Json.JsonException">Файл пошкоджено.</exception>
    public static JsonOrderRepository Open(string dataDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        string filePath = Path.Combine(dataDirectory, FileName);
        string seedPath = Path.Combine(dataDirectory, SeedFileName);
        if (!File.Exists(filePath) && File.Exists(seedPath))
        {
            File.Copy(seedPath, filePath);
        }

        Dictionary<int, Order> orders = JsonFiles.ReadList<OrderRecord>(filePath)
            .Select(OrderRecordMapper.ToDomain)
            .ToDictionary(order => order.Id);
        return new JsonOrderRepository(filePath, orders);
    }

    /// <inheritdoc/>
    public int NextId()
    {
        return _orders.Count == 0 ? 1 : _orders.Keys.Max() + 1;
    }

    /// <inheritdoc/>
    public void Add(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Замовлення №{order.Id} вже є у сховищі.");
        }
    }

    /// <inheritdoc/>
    public Order? FindById(int id)
    {
        return _orders.TryGetValue(id, out Order? order) ? order : null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<Order> ListAll()
    {
        return _orders.Values.ToList();
    }

    /// <inheritdoc/>
    /// <exception cref="IOException">Файл недоступний для запису.</exception>
    public void SaveChanges()
    {
        // TODO(TD-07): файл пишеться цілком і без блокування; два екземпляри втратять зміни.
        List<OrderRecord> records = _orders.Values
            .OrderBy(order => order.Id)
            .Select(OrderRecordMapper.ToRecord)
            .ToList();
        JsonFiles.WriteList(_filePath, records);
    }
}
