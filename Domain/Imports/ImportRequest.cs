using Microsoft.AspNetCore.Http;

namespace Domain.Imports;

public class ImportRequest
{
    public required IFormFile File { get; set; }
}
