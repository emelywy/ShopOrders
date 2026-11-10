namespace ShopOrders.Tests;

using ShopOrders.Domain;
using ShopOrders.Infrastructure;

/// <summary>
/// Інфраструктура: збереження й відновлення замовлень у файлі JSON.
/// Код інфраструктури не входить до вимірювання покриття, але перевіряється.
/// </summary>
public sealed class JsonStorageTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), "shoporders-tests-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void SaveChanges_ThenOpen_RestoresStatusDiscountAndPaidAt()
    {
        JsonOrderRepository repository = JsonOrderRepository.Open(_directory);
        Order order = new(repository.NextId(), 2, new DateOnly(2026, 10, 1));
        order.AddLine(new OrderLine("COFFEE", 2, 250m));
        order.ApplyDiscount(15m);
        order.Pay(485m, new DateTimeOffset(2026, 10, 2, 10, 0, 0, TimeSpan.FromHours(3)));
        order.Ship();
        repository.Add(order);

        repository.SaveChanges();
        Order? restored = JsonOrderRepository.Open(_directory).FindById(order.Id);

        Assert.NotNull(restored);
        Assert.Equal(OrderStatus.Shipped, restored.Status);
        Assert.Equal(485m, restored.Total);
        Assert.Equal(order.PaidAt, restored.PaidAt);
    }

    [Fact]
    public void Open_NoWorkingFile_CopiesSeedFile()
    {
        Directory.CreateDirectory(_directory);
        string seedPath = Path.Combine(_directory, JsonOrderRepository.SeedFileName);
        File.WriteAllText(seedPath, "[]");

        JsonOrderRepository repository = JsonOrderRepository.Open(_directory);

        Assert.True(File.Exists(Path.Combine(_directory, JsonOrderRepository.FileName)));
        Assert.Equal(1, repository.NextId());
    }

    [Fact]
    public void LoadCatalog_MissingFile_ThrowsFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => JsonCatalog.Load(_directory));
    }
}
