namespace ShopOrders.Infrastructure;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Спільні правила читання й запису JSON-файлів даних.
/// </summary>
internal static class JsonFiles
{
    /// <summary>
    /// Налаштування серіалізації: camelCase, відступи, кирилиця без екранування,
    /// перелічення як рядки.
    /// </summary>
    internal static JsonSerializerOptions Options { get; } = CreateOptions();

    /// <summary>
    /// Читає список записів; відсутній файл означає порожній список.
    /// </summary>
    /// <typeparam name="T">Тип запису файлу.</typeparam>
    /// <param name="path">Шлях до JSON-файлу.</param>
    /// <returns>Прочитані записи.</returns>
    internal static List<T> ReadList<T>(string path)
    {
        if (!File.Exists(path))
        {
            return new List<T>();
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json, Options) ?? new List<T>();
    }

    /// <summary>
    /// Записує список записів у файл цілком, створюючи теку за потреби.
    /// </summary>
    /// <typeparam name="T">Тип запису файлу.</typeparam>
    /// <param name="path">Шлях до JSON-файлу.</param>
    /// <param name="items">Записи для збереження.</param>
    internal static void WriteList<T>(string path, IReadOnlyCollection<T> items)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, JsonSerializer.Serialize(items, Options));
    }

    private static JsonSerializerOptions CreateOptions()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
