using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.ViewModels
{
    partial class ImageViewModel
    {
        public interface IMessageService
        {
            void ShowInfo(string text);
            void ShowError(string text);
            void ShowDate(DateTime dateTime);
        }
    }
}
