#pragma warning disable CA1000

namespace ShopOrders.Core.Errors;

/// <summary>
/// Тип-результат для очікуваних операцій без викидання винятків.
/// </summary>
/// <typeparam name="T">Тип значення у разі успіху.</typeparam>
public readonly struct Result<T>
{
    private Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Отримує ознаку успішного виконання операції.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Отримує значення операції.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Отримує повідомлення про помилку.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Створює успішний результат із даними.
    /// </summary>
    /// <param name="value">Значення операції.</param>
    /// <returns>Екземпляр успішного результату.</returns>
    public static Result<T> Ok(T value) => new(true, value, string.Empty);

    /// <summary>
    /// Створює результат з помилкою.
    /// </summary>
    /// <param name="error">Повідомлення про помилку.</param>
    /// <returns>Екземпляр помилкового результату.</returns>
    public static Result<T> Fail(string error) => new(false, default, error);
}

#pragma warning restore CA1000
