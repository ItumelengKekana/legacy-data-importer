using Application.Common.Interfaces;
using Application.Orders.Commands.CreateOrder;
using Application.Orders.Queries.GetOrderById;
using Domain.Customers;
using Domain.Orders;
using Domain.Orders.Response;
using Infrastructure.Services;
using Moq;

namespace LegacyDataImporterTests;

//Tests for the FileParser service
[TestFixture]
public class FileParserTests
{
    private IFileParser _parser = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new FileParser();
    }

    //Tests parsing of a valid legacy customer line with correct format and all required fields.
    //Valid line format: 10 chars (LegacyId) + 30 chars (Name) + 30 chars (Email) + 8 chars (SignupDate YYYYMMDD) + 2 chars (Tier) = 80 total
    [Test]
    public async Task ParseValidLine_ShouldReturnParsedCustomerDto()
    {
        // Arrange
        const string validLine = "0000012345Smith John                    john.smith@email.com          20200115A ";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(validLine);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var results = new List<(int, string, ParsedCustomerDto?, string?)>();
        await foreach (var result in _parser.ParseStreamAsync(stream))
        {
            results.Add(result);
        }

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        var (lineNumber, rawLine, customer, error) = results[0];

        Assert.That(lineNumber, Is.EqualTo(1));
        Assert.That(error, Is.Null);
        Assert.That(customer, Is.Not.Null);
        Assert.That(customer!.LegacyCustomerId, Is.EqualTo("0000012345"));
        Assert.That(customer.FullName, Is.EqualTo("Smith John"));
        Assert.That(customer.Email, Is.EqualTo("john.smith@email.com"));
        Assert.That(customer.SignupDate, Is.EqualTo(DateOnly.ParseExact("20200115", "yyyyMMdd")));
        Assert.That(customer.Tier, Is.EqualTo(Tier.A));
    }

    //Tests parsing of an invalid legacy customer line that is too short.
    [Test]
    public async Task ParseInvalidLine_TooShort_ShouldReturnError()
    {
        // Arrange
        const string invalidLine = "SHORT";  // Less than 80 characters
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(invalidLine);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var results = new List<(int, string, ParsedCustomerDto?, string?)>();
        await foreach (var result in _parser.ParseStreamAsync(stream))
        {
            results.Add(result);
        }

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        var (lineNumber, rawLine, customer, error) = results[0];

        Assert.That(lineNumber, Is.EqualTo(1));
        Assert.That(customer, Is.Null);
        Assert.That(error, Is.Not.Null);
        Assert.That(error, Does.Contain("shorter than expected"));
    }

    [Test]
    public async Task ParseInvalidLine_MissingCustomerId_ShouldReturnError()
    {
        // Arrange
        var invalidLine = "          Smith John                    john.smith@email.com          20200115A ";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(invalidLine);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var results = new List<(int, string, ParsedCustomerDto?, string?)>();
        await foreach (var result in _parser.ParseStreamAsync(stream))
        {
            results.Add(result);
        }

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        var (lineNumber, rawLine, customer, error) = results[0];

        Assert.That(customer, Is.Null);
        Assert.That(error, Does.Contain("LegacyCustomerId is missing"));
    }

    [Test]
    public async Task ParseInvalidLine_InvalidDateFormat_ShouldReturnError()
    {
        // Arrange
        // Line with invalid date format (20-01-15 instead of 20200115)
        const string invalidLine = "0000012345Smith John                    john.smith@email.com          20-01-15A ";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(invalidLine);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var results = new List<(int, string, ParsedCustomerDto?, string?)>();
        await foreach (var result in _parser.ParseStreamAsync(stream))
        {
            results.Add(result);
        }

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        var (lineNumber, rawLine, customer, error) = results[0];

        Assert.That(customer, Is.Null);
        Assert.That(error, Does.Contain("Invalid SignupDate format"));
    }

    [Test]
    public async Task ParseInvalidLine_InvalidTier_ShouldReturnError()
    {
        // Arrange
        // Line with invalid Tier value (F instead of A, B, or C)
        var invalidLine = "0000012345Smith John                    john.smith@email.com          20200115F ";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync(invalidLine);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var results = new List<(int, string, ParsedCustomerDto?, string?)>();
        await foreach (var result in _parser.ParseStreamAsync(stream))
        {
            results.Add(result);
        }

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        var (lineNumber, rawLine, customer, error) = results[0];

        Assert.That(customer, Is.Null);
        Assert.That(error, Does.Contain("Invalid Tier"));
    }
}

//order calculation tests
[TestFixture]
public class OrderTotalCalculationTests
{
    [Test]
    public void CalculateOrderTotal_MultipleItems_ShouldReturnCorrectSum()
    {
        // Arrange
        var items = new List<OrderItemDto>
        {
            new(1, "Widget A", "SKU-001", 10.50f, 2, 21.00f),  // 10.50 * 2 = 21.00
            new(2, "Widget B", "SKU-002", 15.99f, 3, 47.97f),  // 15.99 * 3 = 47.97
            new(3, "Widget C", "SKU-003", 5.00f, 1, 5.00f)     // 5.00 * 1 = 5.00
        };

        // Act
        double totalAmount = Math.Round(items.Sum(i => i.LineTotal), 2);

        // Assert
        Assert.That(totalAmount, Is.EqualTo(73.97).Within(0.01));
    }

    [Test]
    public void CalculateOrderTotal_WithDecimalValues_ShouldRoundCorrectly()
    {
        // Arrange
        var items = new List<OrderItemDto>
        {
            new(1, "Item 1", "SKU-001", 9.99f, 1, 9.99f),      // 9.99
            new(2, "Item 2", "SKU-002", 4.44f, 2, 8.88f),      // 4.44 * 2 = 8.88
            new(3, "Item 3", "SKU-003", 0.05f, 1, 0.05f)       // 0.05
        };

        // Act
        double totalAmount = Math.Round(items.Sum(i => i.LineTotal), 2);

        // Assert
        Assert.That(totalAmount, Is.EqualTo(18.92).Within(0.01));
    }

    [Test]
    public void CalculateOrderTotal_SingleItem_ShouldReturnItemTotal()
    {
        // Arrange
        var items = new List<OrderItemDto>
        {
            new(1, "Single Item", "SKU-001", 99.99f, 1, 99.99f)
        };

        // Act
        double totalAmount = Math.Round(items.Sum(i => i.LineTotal), 2);

        // Assert
        Assert.That(totalAmount, Is.EqualTo(99.99).Within(0.01));
    }

    [Test]
    public void CalculateOrderTotal_WithZeroQuantity_ShouldNotContributeToTotal()
    {
        // Arrange
        var items = new List<OrderItemDto>
        {
            new(1, "Item 1", "SKU-001", 50.00f, 1, 50.00f),
            new(2, "Item 2", "SKU-002", 25.00f, 0, 0.00f)      // Zero quantity
        };

        // Act
        double totalAmount = Math.Round(items.Sum(i => i.LineTotal), 2);

        // Assert
        Assert.That(totalAmount, Is.EqualTo(50.00).Within(0.01));
    }
}

//This is a lightweight integration test that validates the create-retrieve flow.
[TestFixture]
public class OrderApiPathwayTests
{
    private Mock<IOrderRepository> _mockOrderRepository = null!;
    private Mock<ICustomerRepository> _mockCustomerRepository = null!;
    private CreateOrderCommandHandler _createOrderHandler = null!;
    private GetOrderByIdQueryHandler _getOrderByIdHandler = null!;

    [SetUp]
    public void Setup()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _createOrderHandler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockCustomerRepository.Object);
        _getOrderByIdHandler = new GetOrderByIdQueryHandler(_mockOrderRepository.Object);
    }

    [Test]
    public async Task CreateOrder_ThenRetrieveById_ShouldReturnCreatedOrder()
    {
        // Arrange - Create an order
        var createCommand = new CreateOrderCommand(
            CustomerId: 1,
            LegacyCustomerId: null,
            OrderDate: DateTime.Now,
            Currency: "USD",
            Status: "Pending",
            Items:
            [
                new CreateOrderItemDto("Laptop", "SKU-001", 999.99f, 1),
                new CreateOrderItemDto("Mouse", "SKU-002", 49.99f, 2)
            ]
        );

        var createdOrder = new Order
        {
            Id = 1,
            CustomerId = 1,
            OrderDate = createCommand.OrderDate,
            Currency = "USD",
            Status = "Pending"
        };

        var createdItems = new List<OrderItem>
        {
            new() { Id = 1, Description = "Laptop", Sku = "SKU-001", UnitPrice = 999.99m, Quantity = 1 },
            new() { Id = 2, Description = "Mouse", Sku = "SKU-002", UnitPrice = 49.99m, Quantity = 2 }
        };

        _mockOrderRepository.Setup(x => x.AddOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act - Create the order
        var createResult = await _createOrderHandler.Handle(createCommand, CancellationToken.None);

        // Assert - Create operation successful
        Assert.That(createResult.Success, Is.True);
        Assert.That(createResult.Message, Does.Contain("created successfully"));

        // Arrange - Setup mock
        var orderDetailsResult = new OrderDetailsResponse(
            Order: createdOrder,
            CustomerName: "John Doe",
            Items: createdItems
        );

        _mockOrderRepository.Setup(x => x.GetOrderDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderDetailsResult);

        // Act - Retrieve the created order
        var getQuery = new GetOrderByIdQuery(1);
        var retrievedOrder = await _getOrderByIdHandler.Handle(getQuery, CancellationToken.None);

        // Assert - Retrieve operation successful and data is correct
        Assert.That(retrievedOrder, Is.Not.Null);
        Assert.That(retrievedOrder!.Id, Is.EqualTo(1));
        Assert.That(retrievedOrder.CustomerId, Is.EqualTo(1));
        Assert.That(retrievedOrder.CustomerName, Is.EqualTo("John Doe"));
        Assert.That(retrievedOrder.Currency, Is.EqualTo("USD"));
        Assert.That(retrievedOrder.Status, Is.EqualTo("Pending"));
        Assert.That(retrievedOrder.Items, Has.Count.EqualTo(2));

        // Verify order total is calculated correctly
        var expectedTotal = Math.Round(999.99 + (49.99 * 2), 2);
        Assert.That(retrievedOrder.TotalAmount, Is.EqualTo(expectedTotal).Within(0.01));
    }

    [Test]
    public async Task CreateOrder_WithLegacyCustomerId_ShouldResolveCustomer()
    {
        // Arrange
        const string legacyCustomerId = "0000012345";
        var customer = new Customer { Id = 5, LegacyCustomerId = legacyCustomerId };

        var createCommand = new CreateOrderCommand(
            CustomerId: null,
            LegacyCustomerId: legacyCustomerId,
            OrderDate: DateTime.Now,
            Currency: "EUR",
            Status: "Processing",
            Items:
            [
                new CreateOrderItemDto("Product A", "SKU-A01", 29.99f, 1)
            ]
        );

        _mockCustomerRepository.Setup(x => x.GetByLegacyIdAsync(legacyCustomerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        _mockOrderRepository.Setup(x => x.AddOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _createOrderHandler.Handle(createCommand, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _mockCustomerRepository.Verify(x => x.GetByLegacyIdAsync(legacyCustomerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RetrieveOrder_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        _mockOrderRepository.Setup(x => x.GetOrderDetailsByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderDetailsResponse?)null);

        // Act
        var query = new GetOrderByIdQuery(999);
        var result = await _getOrderByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}
