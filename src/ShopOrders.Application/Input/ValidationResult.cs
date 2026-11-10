namespace ShopOrders.Application.Input;

/// <summary>
/// Результат перевірки введеного користувачем значення. Некоректний ввід — очікувана
/// ситуація, тому повідомляється цим типом, а не винятком.
/// </summary>
public sealed class ValidationResult
{
    private ValidationResult(bool isValid, string errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Спільний екземпляр успішної перевірки.
    /// </summary>
    public static ValidationResult Success { get; } = new(true, string.Empty);

    /// <summary>
    /// Ознака того, що значення пройшло перевірку.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Пояснення для користувача; порожній рядок, якщо перевірка успішна.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    /// Створює результат невдалої перевірки.
    /// </summary>
    /// <param name="errorMessage">Пояснення, що саме не так і як виправити.</param>
    /// <returns>Результат з <see cref="IsValid"/>, рівним <see langword="false"/>.</returns>
    public static ValidationResult Fail(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
        return new ValidationResult(false, errorMessage);
    }
}
