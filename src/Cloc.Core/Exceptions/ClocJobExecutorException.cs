namespace Cloc;

public class ClocJobExecutorException : ClocException
{
    public ClocJobExecutorException(string msgTemplate, string jobId)
        : base(string.Format(msgTemplate, jobId))
    {

    }
}
