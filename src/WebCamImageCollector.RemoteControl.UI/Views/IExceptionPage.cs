using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Views
{
    public interface IExceptionPage
    {
        bool TryProcess(Exception e);
    }
}
