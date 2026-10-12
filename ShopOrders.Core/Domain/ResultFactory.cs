namespace ShopOrders.Core.Domain;

/// <summary>
/// Фабричні методи для створення результатів операцій.
/// </summary>
public static class ResultFactory
{
    /// <summary>
    /// Створює успішний результат із даними.
    /// </summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення операції.</param>
    /// <returns>Екземпляр успішного результату.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, null);

    /// <summary>
    /// Створює результат з помилкою.
    /// </summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="error">Повідомлення про помилку.</param>
    /// <returns>Екземпляр помилкового результату.</returns>
    public static Result<T> Fail<T>(string error) => new(false, default, error);
}
