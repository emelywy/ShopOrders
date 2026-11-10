namespace ShopOrders.Tests;

using ShopOrders.Application;
using ShopOrders.Domain;

/// <summary>
/// Правило залежностей: стрілки спрямовані всередину, домен не знає ні про кого.
/// Компілятор уже забороняє зворотні посилання; тести фіксують правило явно.
/// </summary>
public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_DoesNotReferenceOtherProjects()
    {
        IEnumerable<string> links = ProjectReferencesOf(typeof(Order));

        Assert.Empty(links);
    }

    [Fact]
    public void Application_ReferencesOnlyDomain()
    {
        List<string> links = ProjectReferencesOf(typeof(OrderService)).ToList();

        Assert.Equal("ShopOrders.Domain", Assert.Single(links));
    }

    private static IEnumerable<string> ProjectReferencesOf(Type type)
    {
        return type.Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name ?? string.Empty)
            .Where(name => name.StartsWith("ShopOrders", StringComparison.Ordinal));
    }
}
