namespace Cloc.Core.Exceptions
{
    public class ClocException : Exception
    {
        public ClocException(Exception innerException)
            : base("Cloc exception occured", innerException) { }

        public ClocException(string message, Exception innerException)
            : base(message, innerException) { }

        public ClocException(string message)
            : base(message)
        {

        }
    }
}
