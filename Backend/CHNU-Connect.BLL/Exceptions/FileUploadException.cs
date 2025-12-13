using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Exceptions
{
    public class FileUploadException : Exception
    {
        public FileUploadException() : base("File upload failed.") { }
    }
}
