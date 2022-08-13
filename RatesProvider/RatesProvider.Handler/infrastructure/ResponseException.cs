namespace RatesProvider.Handler.infrastructure;

public class ResponseException : Exception
{
    public ResponseException(string message)
    : base(message) { }
}
