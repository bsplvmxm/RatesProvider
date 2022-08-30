namespace RatesProvider.Recipient.Exceptions;

public class RatesBuildException : Exception
{
    public RatesBuildException(string message)
    : base(message) { }
}
