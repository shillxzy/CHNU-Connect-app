using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Exceptions
{
    public class PasswordTooWeakException : Exception
    {
        public PasswordTooWeakException() : base("Password must contain at least 8 characters.") { }
    }
}
