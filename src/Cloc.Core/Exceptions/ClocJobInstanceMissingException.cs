namespace Cloc.Core.Exceptions
{
    public class ClocJobInstanceMissingException : ClocException
    {
        public ClocJobInstanceMissingException(string id)
            : base($"Executor could not find registered job with ID: {id}")
        {

        }
    }
}
