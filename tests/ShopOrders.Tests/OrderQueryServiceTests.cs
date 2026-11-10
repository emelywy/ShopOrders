namespace ShopOrders.Tests;

using ShopOrders.Application;
using ShopOrders.Application.Reports;
using ShopOrders.Domain;
using ShopOrders.Tests.Doubles;

/// <summary>
/// Пошук UC-05 і звіт «Продажі за період» UC-06: порожній результат, один запис,
/// кілька записів у правильному порядку.
/// </summary>
public sealed class OrderQueryServiceTests
{
    private readonly FakeOrderRepository _orders = new();
    private readonly OrderQueryService _queries;

    public OrderQueryServiceTests()
    {
        _queries = new OrderQueryService(_orders, new SpyLog());
    }

    [Fact]
    public void Search_NoMatches_ReturnsEmpty()
    {
        _orders.Add(NewOrder(1, 1, Day(5)));

        OrderSearchCriteria criteria = new()
        {
            CustomerId = 2,
        };

        IReadOnlyList<Order> found = _queries.Search(criteria);

        Assert.Empty(found);
    }

    [Fact]
    public void Search_ByCustomerAndStatus_ReturnsSingle()
    {
        _orders.Add(PaidOrder(1, 1, Day(5), Day(5)));
        _orders.Add(NewOrder(2, 1, Day(6)));
        _orders.Add(PaidOrder(3, 2, Day(6), Day(6)));
        OrderSearchCriteria criteria = new()
        {
            CustomerId = 1,
            Status = OrderStatus.Paid,
        };

        IReadOnlyList<Order> found = _queries.Search(criteria);

        Order single = Assert.Single(found);
        Assert.Equal(1, single.Id);
    }

    [Fact]
    public void Search_SeveralMatches_SortedByDateThenId()
    {
        _orders.Add(NewOrder(3, 1, Day(5)));
        _orders.Add(NewOrder(1, 1, Day(7)));
        _orders.Add(NewOrder(2, 1, Day(5)));

        IReadOnlyList<Order> found = _queries.Search(new OrderSearchCriteria());

        Assert.Equal("2,3,1", string.Join(',', found.Select(order => order.Id)));
    }

    [Fact]
    public void Search_ByPeriod_ExcludesOrdersOutsidePeriod()
    {
        _orders.Add(NewOrder(1, 1, Day(1)));
        _orders.Add(NewOrder(2, 1, Day(10)));
        _orders.Add(NewOrder(3, 1, Day(20)));
        OrderSearchCriteria criteria = new()
        {
            CreatedFrom = Day(10),
            CreatedTo = Day(10),
        };

        IReadOnlyList<Order> found = _queries.Search(criteria);

        Assert.Equal(2, Assert.Single(found).Id);
    }

    [Fact]
    public void BuildSalesReport_NoSales_IsEmpty()
    {
        _orders.Add(NewOrder(1, 1, Day(2)));
        Order cancelled = PaidOrder(2, 1, Day(2), Day(2));
        cancelled.Cancel();
        _orders.Add(cancelled);

        SalesReport report = _queries.BuildSalesReport(Day(1), Day(31));

        Assert.True(report.IsEmpty);
        Assert.Equal(0m, report.Revenue);
    }

    [Fact]
    public void BuildSalesReport_SingleSale_HasOneRow()
    {
        _orders.Add(PaidOrder(1, 1, Day(3), Day(4)));

        SalesReport report = _queries.BuildSalesReport(Day(1), Day(31));

        SalesReportRow row = Assert.Single(report.Rows);
        Assert.Equal(Day(4), row.Date);
        Assert.Equal(1, row.OrderCount);
        Assert.Equal(250m, row.Revenue);
    }

    [Fact]
    public void BuildSalesReport_SeveralDays_RowsOrderedWithTotals()
    {
        _orders.Add(PaidOrder(1, 1, Day(3), Day(3)));
        _orders.Add(PaidOrder(2, 2, Day(3), Day(3)));
        Order shipped = PaidOrder(3, 3, Day(1), Day(1));
        shipped.Ship();
        _orders.Add(shipped);
        _orders.Add(PaidOrder(4, 1, Day(20), Day(20)));

        SalesReport report = _queries.BuildSalesReport(Day(1), Day(10));

        Assert.Equal("1,3", string.Join(',', report.Rows.Select(row => row.Date.Day)));
        Assert.Equal(2, report.Rows[1].OrderCount);
        Assert.Equal(3, report.OrderCount);
        Assert.Equal(750m, report.Revenue);
    }

    [Fact]
    public void BuildSalesReport_EndBeforeStart_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _queries.BuildSalesReport(Day(10), Day(1)));
    }

    private static DateOnly Day(int dayOfOctober)
    {
        return new DateOnly(2026, 10, dayOfOctober);
    }

    private static Order NewOrder(int id, int customerId, DateOnly createdAt)
    {
        Order order = new(id, customerId, createdAt);
        order.AddLine(new OrderLine("COFFEE", 1, 250m));
        return order;
    }

    private static Order PaidOrder(int id, int customerId, DateOnly createdAt, DateOnly paidOn)
    {
        Order order = NewOrder(id, customerId, createdAt);
        DateTimeOffset paidAt = new(paidOn.ToDateTime(new TimeOnly(12, 0)), TimeSpan.FromHours(3));
        order.Pay(order.Total, paidAt);
        return order;
    }
}
