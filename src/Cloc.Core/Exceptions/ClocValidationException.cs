namespace Cloc;

public class ClocValidationException : ClocException
{
    public ClocValidationException(string message)
        : base("Cloc validation exception occured. " + message) { }

    public ClocValidationException(string message, Exception innerException)
        : base("Cloc validation exception occured. " + message, innerException) { }
}
