﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.ViewModels
{
    public interface IClientStatusViewModel
    {
        bool IsRunning { get; set; }
        bool IsStatusLoading { get; set; }
    }
}
