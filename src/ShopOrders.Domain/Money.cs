namespace ShopOrders.Domain;

/// <summary>
/// Правило округлення грошових сум, спільне для всього домену.
/// </summary>
public static class Money
{
    /// <summary>
    /// Кількість знаків після коми в грошовій сумі (копійки).
    /// </summary>
    public const int Decimals = 2;

    /// <summary>
    /// Округлює суму до копійок; половина копійки округлюється від нуля (0,005 → 0,01).
    /// </summary>
    /// <param name="amount">Сума в гривнях з довільною точністю.</param>
    /// <returns>Сума, округлена до двох знаків після коми.</returns>
    public static decimal Round(decimal amount)
    {
        return decimal.Round(amount, Decimals, MidpointRounding.AwayFromZero);
    }
}
