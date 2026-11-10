namespace ShopOrders.Infrastructure;

using ShopOrders.Application.Abstractions;
using ShopOrders.Domain;
using ShopOrders.Infrastructure.Records;

/// <summary>
/// Довідники товарів і покупців, прочитані з файлів JSON теки даних.
/// </summary>
public sealed class JsonCatalog : ICatalog
{
    /// <summary>
    /// Ім'я файлу каталогу товарів.
    /// </summary>
    public const string ProductsFileName = "products.json";

    /// <summary>
    /// Ім'я файлу довідника покупців.
    /// </summary>
    public const string CustomersFileName = "customers.json";

    private readonly Dictionary<string, Product> _products;
    private readonly Dictionary<int, Customer> _customers;

    private JsonCatalog(Dictionary<string, Product> products, Dictionary<int, Customer> customers)
    {
        _products = products;
        _customers = customers;
    }

    /// <summary>
    /// Кількість товарів у каталозі.
    /// </summary>
    public int ProductCount => _products.Count;

    /// <summary>
    /// Кількість покупців у довіднику.
    /// </summary>
    public int CustomerCount => _customers.Count;

    /// <summary>
    /// Завантажує обидва довідники з теки даних.
    /// </summary>
    /// <param name="dataDirectory">Тека з файлами products.json і customers.json.</param>
    /// <returns>Завантажені довідники.</returns>
    /// <exception cref="FileNotFoundException">Файлу довідника немає.</exception>
    /// <exception cref="System.Text.Json.JsonException">Файл пошкоджено.</exception>
    public static JsonCatalog Load(string dataDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        List<ProductRecord> productRecords =
            ReadRequired<ProductRecord>(dataDirectory, ProductsFileName);
        List<CustomerRecord> customerRecords =
            ReadRequired<CustomerRecord>(dataDirectory, CustomersFileName);
        Dictionary<string, Product> products = productRecords
            .Select(item => new Product(item.Sku, item.Title, item.Price))
            .ToDictionary(product => product.Sku, StringComparer.OrdinalIgnoreCase);
        Dictionary<int, Customer> customers = customerRecords
            .Select(item => new Customer(item.Id, item.FullName, item.Status))
            .ToDictionary(customer => customer.Id);
        return new JsonCatalog(products, customers);
    }

    /// <inheritdoc/>
    public Product? FindProduct(string sku)
    {
        ArgumentNullException.ThrowIfNull(sku);
        return _products.TryGetValue(sku, out Product? product) ? product : null;
    }

    /// <inheritdoc/>
    public Customer? FindCustomer(int id)
    {
        return _customers.TryGetValue(id, out Customer? customer) ? customer : null;
    }

    private static List<T> ReadRequired<T>(string dataDirectory, string fileName)
    {
        string path = Path.Combine(dataDirectory, fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Не знайдено файл довідника {path}.", path);
        }

        return JsonFiles.ReadList<T>(path);
    }
}
