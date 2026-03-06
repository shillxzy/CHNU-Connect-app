using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Exceptions
{
    public class PostNotFoundException : Exception
    {
        public PostNotFoundException() : base("Post not found.") { }
    }
}
