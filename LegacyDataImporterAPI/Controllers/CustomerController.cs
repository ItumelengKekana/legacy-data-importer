using Application.Customers.Commands.ImportCustomers;
using Application.Orders.Commands.CreateOrder;
using Application.Orders.Queries.GetCustomerOrderSummary;
using Application.Orders.Queries.GetOrderById;
using Domain.Imports;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LegacyDataImporterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("import-legacy-data")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Import([FromForm][Required] ImportRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("A valid non-empty file is required.");
        }

        await using var stream = request.File.OpenReadStream();
        var command = new ImportCustomersCommand(request.File);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("create-order")]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody][Required] CreateOrderCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("GetOrderById")]
    [ProducesResponseType(typeof(OrderDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById([Required] int id, CancellationToken ct)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id), ct);
        return order != null ? Ok(order) : NotFound();
    }

    [HttpGet("order-summary")]
    [ProducesResponseType(typeof(List<CustomerOrderSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderSummaries(
        [FromQuery][Required] string fromDate,
        [FromQuery][Required] string toDate,
        CancellationToken ct)
    {
        var query = new GetCustomerOrderSummaryQuery(fromDate, toDate);
        var result = await _mediator.Send(query, ct);

        //if (result.Count == 0)
        //{
        //    return NotFound("No orders found for the specified date range.");
        //}

        return Ok(result);
    }

}
