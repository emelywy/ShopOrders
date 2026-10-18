// Виправлено стиль коду за вимогами аналізаторів

using System;

namespace ShopOrders.Core.Domain;

/// <summary>
/// Правила розрахунку знижок.
/// </summary>
public sealed class DiscountPolicy
{
    /// <summary>Знижка постійному клієнтові.</summary>
    public const decimal RegularRate = 0.05m;

    /// <summary>Додаткова знижка за велике замовлення.</summary>
    public const decimal BigOrderRate = 0.10m;

    /// <summary>Межа великого замовлення, грн.</summary>
    public const decimal BigOrderFrom = 1000m;

    /// <summary>Максимальна сума знижки, грн.</summary>
    public const decimal MaxDiscount = 150m;

    /// <summary>
    /// Розраховує знижку.
    /// </summary>
    /// <param name="amount">Сума замовлення.</param>
    /// <param name="isRegular">Чи є клієнт постійним.</param>
    /// <returns>Розмір знижки.</returns>
#pragma warning disable CA1822
    public decimal DiscountFor(decimal amount, bool isRegular)
#pragma warning restore CA1822
    {
        EnsureAmountIsValid(amount);
        decimal raw = amount * RateFor(amount, isRegular);
        return Math.Min(decimal.Round(raw, 2), MaxDiscount);
    }

    /// <summary>
    /// Перевіряє, чи сума є коректною.
    /// </summary>
    /// <param name="amount">Сума для перевірки.</param>
    private static void EnsureAmountIsValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від’ємною.");
        }
    }

    /// <summary>
    /// Визначає відсоткову ставку знижки.
    /// </summary>
    /// <param name="amount">Сума замовлення.</param>
    /// <param name="isRegular">Чи є клієнт постійним.</param>
    /// <returns>Ставка знижки.</returns>
    private static decimal RateFor(decimal amount, bool isRegular)
    {
        decimal rate = isRegular ? RegularRate : 0m;
        if (amount >= BigOrderFrom)
        {
            rate += BigOrderRate;
        }

        return rate;
    }
}
