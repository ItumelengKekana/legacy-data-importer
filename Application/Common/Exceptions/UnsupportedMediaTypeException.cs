namespace Application.Common.Exceptions;

public class UnsupportedMediaTypeException(string message) : Exception($"{message}")
{
}
