using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    partial class DownloadImageCommand
    {
        public interface IViewModel
        {
            ImageQuality Quality { get; }
            bool IsDownloading { get; set; }
        }
    }
}
