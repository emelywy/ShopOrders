namespace ShopOrders.Core.Reporting;

using System;
using System.IO;

/// <summary>
/// Журнал аудиту дій користувача з гарантованим звільненням файлового ресурсу.
/// </summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="FileAuditLog"/>.
    /// </summary>
    /// <param name="path">Шлях до файлу журналу аудиту.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>
    /// Записує подію аудиту у файл.
    /// </summary>
    /// <param name="operation">Назва виконаної операції.</param>
    /// <param name="documentId">Ідентифікатор документа/замовлення.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _writer.WriteLine($"{DateTimeOffset.Now:O};{operation};{documentId}");
    }

    /// <summary>
    /// Звільняє файловий потік та зберігає буферизовані дані.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
