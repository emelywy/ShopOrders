namespace ShopOrders.Core.Domain;

/// <summary>
/// Результат операции: либо успешное значение, либо сообщение об ошибке.
/// </summary>
/// <typeparam name="T">Тип значения в случае успеха.</typeparam>
public sealed class Result<T>
{
    private readonly T? _value;

    internal Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        Error = error;
    }

    /// <summary>
    /// Получает признак успешности операции.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Получает признак неудачи операции.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Получает текст сообщения об ошибке.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Получает полезное значение в случае успеха.
    /// </summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Неможливо отримати значення для помилкового результату.");
}
