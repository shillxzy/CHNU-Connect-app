using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException() : base("Unexpected error occurred.") { }
    }
}
