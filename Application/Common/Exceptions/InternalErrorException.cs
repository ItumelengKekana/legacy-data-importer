namespace Application.Common.Exceptions;

public class InternalErrorException(string message) : Exception($"{message}")
{
}
