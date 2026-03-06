using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException() : base("Session expired. Please log in again.") { }
    }
}
