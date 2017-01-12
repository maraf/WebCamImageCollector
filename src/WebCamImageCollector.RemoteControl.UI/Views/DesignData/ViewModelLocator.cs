using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.ViewModels;

namespace WebCamImageCollector.RemoteControl.Views.DesignData
{
    internal class ViewModelLocator
    {
        private OverviewViewModel overview;
        public OverviewViewModel Overview
        {
            get
            {
                if (overview == null)
                {
                    overview = new OverviewViewModel();
                    overview.Remotes.Add(new ClientOverviewViewModel(new RemoteClient(Guid.NewGuid(), "Home", "http://home:8001", "xxxxx")));
                    overview.Remotes.Add(new ClientOverviewViewModel(new RemoteClient(Guid.NewGuid(), "Office", "http://office:8001", "zzzzz")));
                }

                return overview;
            }
        }
    }
}
