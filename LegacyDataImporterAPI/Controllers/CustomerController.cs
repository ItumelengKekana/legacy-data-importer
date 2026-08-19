using Application.Customers.Commands.ImportCustomers;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace LegacyDataImporterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
	private readonly IMediator _mediator = mediator;

	[HttpPost("import")]
	[Consumes("multipart/form-data")]
	[ProducesResponseType(typeof(ImportSummaryDto), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Import([FromForm] IFormFile file, CancellationToken cancellationToken)
	{
		if (file is null || file.Length == 0)
		{
			return BadRequest("A valid non-empty file is required for import.");
		}

		await using var stream = file.OpenReadStream();
		//var command = new ImportCustomersCommandHandler(stream);

		var response = await _mediator.Send(stream, cancellationToken);

		return Ok(response);
	}
}
