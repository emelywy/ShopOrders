namespace ShopOrders.Core.Legacy;

/// <summary>
/// "Товстий" інтерфейс сховища, що порушує ISP.
/// </summary>
public interface IOrderStore
{
    /// <summary>Додає замовлення.</summary>
    void Add(LegacyOrder order);

    /// <summary>Отримує замовлення за ID.</summary>
    LegacyOrder GetById(int id);

    /// <summary>Зберігає замовлення у файл.</summary>
    void SaveToFile(LegacyOrder order, string path);

    /// <summary>Надсилає лист.</summary>
    void SendEmail(string to, string body);

    /// <summary>Експортує CSV.</summary>
    string ExportCsv();

    /// <summary>Робить резервну копію.</summary>
    void Backup(string folder);

    /// <summary>Друкує накладну.</summary>
    void PrintInvoice(LegacyOrder order);
}

/// <summary>
/// Конкретний клас файлового сховища для компіляції легасі коду.
/// </summary>
public class FileOrderStore
{
    /// <summary>Зберігає замовлення у файл.</summary>
    public void SaveToFile(LegacyOrder order, string path) { }
}

/// <summary>
/// Конкретний клас поштового відправника для компіляції легасі коду.
/// </summary>
public class SmtpMailer
{
    /// <summary>Надсилає лист.</summary>
    public void Send(string to, string subject, string body) { }
}
