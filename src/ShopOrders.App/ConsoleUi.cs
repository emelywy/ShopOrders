namespace ShopOrders.App;

using ShopOrders.Application.Input;

/// <summary>
/// Обгортка над потоками вводу й виводу; повторює запит, доки значення не пройде барикаду.
/// </summary>
public sealed class ConsoleUi
{
    /// <summary>
    /// Повідомлення про завершення вводу (кінець потоку).
    /// </summary>
    public const string InputClosedMessage = "Ввід завершено.";

    private readonly TextReader _input;
    private readonly TextWriter _output;

    /// <summary>
    /// Створює інтерфейс над заданими потоками; у застосунку це Console.In і Console.Out.
    /// </summary>
    /// <param name="input">Потік вводу.</param>
    /// <param name="output">Потік виводу.</param>
    public ConsoleUi(TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        _input = input;
        _output = output;
    }

    /// <summary>
    /// Виводить рядок тексту.
    /// </summary>
    /// <param name="text">Текст для показу.</param>
    public void WriteLine(string text)
    {
        _output.WriteLine(text);
    }

    /// <summary>
    /// Показує запрошення й читає рядок.
    /// </summary>
    /// <param name="prompt">Запрошення до вводу.</param>
    /// <returns>Введений рядок або <see langword="null"/>, якщо ввід завершено.</returns>
    public string? ReadLine(string prompt)
    {
        _output.Write(prompt);
        return _input.ReadLine();
    }

    /// <summary>
    /// Запитує значення, доки воно не пройде перевірку; помилку показує й питає знову.
    /// </summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="prompt">Запрошення до вводу.</param>
    /// <param name="parse">Правило розбору з барикади <see cref="InputParser"/>.</param>
    /// <returns>Перевірене значення.</returns>
    /// <exception cref="OperationCanceledException">Ввід завершився раніше.</exception>
    public T Ask<T>(string prompt, Func<string?, (ValidationResult Result, T Value)> parse)
    {
        ArgumentNullException.ThrowIfNull(parse);
        while (true)
        {
            string text = ReadLine(prompt)
                ?? throw new OperationCanceledException(InputClosedMessage);
            (ValidationResult result, T value) = parse(text);
            if (result.IsValid)
            {
                return value;
            }

            WriteLine(result.ErrorMessage);
        }
    }
}
