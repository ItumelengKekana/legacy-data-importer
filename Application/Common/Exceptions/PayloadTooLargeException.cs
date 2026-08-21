namespace Application.Common.Exceptions;

public class PayloadTooLargeException(string message) : Exception($"{message}")
{
}
