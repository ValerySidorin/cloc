using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloc.Core.Exceptions
{
    public class ClocJobContextParsingException : ClocException
    {
        public ClocJobContextParsingException(
            Exception innerException)
            : base("Cloc exception occured while parsing context", innerException)
        {
        }
    }
}
