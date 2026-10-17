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
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isRegular)
        {
            rate += RegularRate;
        }

        if (amount >= BigOrderFrom)
        {
            rate += BigOrderRate;
        }

        return decimal.Round(amount * rate, 2);
    }
}
