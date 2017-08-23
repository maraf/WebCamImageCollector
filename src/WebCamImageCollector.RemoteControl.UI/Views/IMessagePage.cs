using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.Views
{
    public interface IMessagePage
    {
        void ShowInfo(string text);
        void ShowError(string text);
    }
}
