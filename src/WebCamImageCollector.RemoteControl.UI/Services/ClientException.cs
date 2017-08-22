using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class ClientException : Exception
    {
        protected ClientException()
        { }

        public ClientException(Exception inner)
            : base("Unspecific exception", inner)
        { }
    }
}
