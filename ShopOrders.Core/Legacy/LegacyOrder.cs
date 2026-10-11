namespace ShopOrders.Core.Legacy;

/// <summary>
/// Застаріла модель рядка замовлення.
/// </summary>
public class LegacyLine
{
    /// <summary>
    /// Отримує або задає артикул.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Отримує або задає кількість.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Отримує або задає ціну.
    /// </summary>
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Застаріла модель замовлення з відкритими сетерами.
/// </summary>
public class LegacyOrder
{
    /// <summary>
    /// Отримує або задає ідентифікатор.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Отримує або задає числовий статус.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Отримує або задає суму.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Отримує або задає email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Отримує або задає список позицій.
    /// </summary>
    public List<LegacyLine> Lines { get; set; } = new();

    /// <summary>
    /// Додає позицію до замовлення.
    /// </summary>
    /// <param name="line">Позиція замовлення.</param>
    public virtual void AddLine(LegacyLine line) => Lines.Add(line);
}

/// <summary>
/// Клас архівного замовлення, що порушує LSP.
/// </summary>
public class ArchivedOrder : LegacyOrder
{
    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="ArchivedOrder"/>.
    /// </summary>
    /// <param name="id">Ідентифікатор.</param>
    public ArchivedOrder(int id) => Id = id;

    /// <summary>
    /// Перевизначений метод, що викидає виняток і порушує контракт базового класу.
    /// </summary>
    /// <param name="line">Позиція замовлення.</param>
    public override void AddLine(LegacyLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
