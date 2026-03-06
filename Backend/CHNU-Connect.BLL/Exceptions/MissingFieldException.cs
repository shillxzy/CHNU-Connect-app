using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Exceptions
{
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string fieldName) : base($"Field '{fieldName}' is required.") { }
    }
}
