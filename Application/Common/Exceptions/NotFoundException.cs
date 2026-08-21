namespace Application.Common.Exceptions;

public class NotFoundException(string name, object key) : Exception($"{name} of ({key}) was not found")
{
}
