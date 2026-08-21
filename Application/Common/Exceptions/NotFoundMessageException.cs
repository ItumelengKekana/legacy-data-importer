namespace Application.Common.Exceptions;

public class NotFoundMessageException(string message) : Exception($"{message}")
{
}
