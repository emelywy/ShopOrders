namespace ShopOrders.Application.Input;

using System.Globalization;
using ShopOrders.Domain;

/// <summary>
/// Барикада валідації: кожне значення з консолі перевіряється й перетворюється
/// на типізоване рівно один раз, на вході до прикладного шару.
/// </summary>
public static class InputParser
{
    /// <summary>
    /// Формат дати, який приймає застосунок.
    /// </summary>
    public const string DateFormat = "dd.MM.yyyy";

    /// <summary>
    /// Найбільша довжина артикула.
    /// </summary>
    public const int MaxSkuLength = 20;

    /// <summary>
    /// Розбирає додатне ціле число (номер, кількість).
    /// </summary>
    /// <param name="text">Введений рядок; може бути <see langword="null"/>.</param>
    /// <returns>Результат перевірки й число (0, якщо перевірка невдала).</returns>
    public static (ValidationResult Result, int Value) ParsePositiveNumber(string? text)
    {
        bool parsed = int.TryParse(
            text?.Trim(),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int value);
        if (!parsed || value <= 0)
        {
            return (ValidationResult.Fail("Очікується ціле число, більше за нуль."), 0);
        }

        return (ValidationResult.Success, value);
    }

    /// <summary>
    /// Розбирає артикул: обрізає пробіли й переводить у верхній регістр.
    /// </summary>
    /// <param name="text">Введений артикул.</param>
    /// <returns>Результат перевірки й нормалізований артикул.</returns>
    public static (ValidationResult Result, string Value) ParseSku(string? text)
    {
        string sku = (text ?? string.Empty).Trim().ToUpperInvariant();
        if (sku.Length == 0 || sku.Length > MaxSkuLength || sku.Any(char.IsWhiteSpace))
        {
            string message = $"Артикул має бути від 1 до {MaxSkuLength} символів без пробілів.";
            return (ValidationResult.Fail(message), string.Empty);
        }

        return (ValidationResult.Success, sku);
    }

    /// <summary>
    /// Розбирає грошову суму; приймає кому або крапку як роздільник.
    /// </summary>
    /// <param name="text">Введена сума, наприклад «249,50».</param>
    /// <returns>Результат перевірки й сума в гривнях.</returns>
    public static (ValidationResult Result, decimal Value) ParseMoney(string? text)
    {
        string normalized = (text ?? string.Empty).Trim().Replace(',', '.');
        bool parsed = decimal.TryParse(
            normalized,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out decimal value);
        if (!parsed || Money.Round(value) != value)
        {
            string message = "Очікується невід'ємна сума з не більше ніж двома знаками після коми.";
            return (ValidationResult.Fail(message), 0m);
        }

        return (ValidationResult.Success, value);
    }

    /// <summary>
    /// Розбирає дату у форматі <see cref="DateFormat"/>.
    /// </summary>
    /// <param name="text">Введена дата, наприклад «01.10.2026».</param>
    /// <returns>Результат перевірки й дата.</returns>
    public static (ValidationResult Result, DateOnly Value) ParseDate(string? text)
    {
        bool parsed = DateOnly.TryParseExact(
            text?.Trim(),
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateOnly value);
        if (!parsed)
        {
            return (ValidationResult.Fail("Очікується наявна дата у форматі дд.мм.рррр."), value);
        }

        return (ValidationResult.Success, value);
    }

    /// <summary>
    /// Розбирає назву стану замовлення без урахування регістру.
    /// </summary>
    /// <param name="text">Назва стану: New, Paid, Shipped або Cancelled.</param>
    /// <returns>Результат перевірки й стан.</returns>
    public static (ValidationResult Result, OrderStatus Value) ParseStatus(string? text)
    {
        string name = (text ?? string.Empty).Trim();
        bool parsed = Enum.TryParse(name, ignoreCase: true, out OrderStatus value);
        if (!parsed || !Enum.IsDefined(value) || name.All(char.IsDigit))
        {
            string message = "Очікується один зі станів: New, Paid, Shipped, Cancelled.";
            return (ValidationResult.Fail(message), OrderStatus.New);
        }

        return (ValidationResult.Success, value);
    }

    /// <summary>
    /// Робить критерій необов'язковим: порожній ввід означає «будь-яке значення».
    /// </summary>
    /// <typeparam name="T">Тип значення критерію.</typeparam>
    /// <param name="text">Введений рядок.</param>
    /// <param name="parse">Правило розбору непорожнього значення.</param>
    /// <returns>Успіх із <see langword="null"/> для порожнього вводу або результат.</returns>
    public static (ValidationResult Result, T? Value) ParseOptional<T>(
        string? text,
        Func<string?, (ValidationResult Result, T Value)> parse)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(parse);
        if (string.IsNullOrWhiteSpace(text))
        {
            return (ValidationResult.Success, null);
        }

        (ValidationResult result, T value) = parse(text);
        T? checkedValue = result.IsValid ? value : null;
        return (result, checkedValue);
    }

    /// <summary>
    /// Перевіряє, що період задано в правильному порядку.
    /// </summary>
    /// <param name="periodStart">Перший день періоду.</param>
    /// <param name="periodEnd">Останній день періоду.</param>
    /// <returns>Успіх, якщо кінець не раніше за початок.</returns>
    public static ValidationResult ValidatePeriod(DateOnly periodStart, DateOnly periodEnd)
    {
        return periodEnd < periodStart
            ? ValidationResult.Fail("Кінець періоду не може бути раніше за початок.")
            : ValidationResult.Success;
    }
}
