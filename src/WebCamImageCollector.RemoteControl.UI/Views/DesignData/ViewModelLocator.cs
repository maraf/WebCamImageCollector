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
                    overview.Clients.Add(new ClientOverviewViewModel(new RemoteClient(Guid.NewGuid(), "Home", "http://home:8001", "xxxxx")));
                    overview.Clients.Add(new ClientOverviewViewModel(new RemoteClient(Guid.NewGuid(), "Office", "http://office:8001", "zzzzz")));
                }

                return overview;
            }
        }

        private RemoteClientEditViewModel remoteClientEdit;
        public RemoteClientEditViewModel RemoteClientEdit
        {
            get
            {
                if (remoteClientEdit == null)
                {
                    remoteClientEdit = new RemoteClientEditViewModel();
                    remoteClientEdit.Name = "Home";
                    remoteClientEdit.Url = "http://localhost/webcam";
                    remoteClientEdit.Name = "abcdefgh";
                }

                return remoteClientEdit;
            }
        }

        private LocalClientEditViewModel localClientEdit;
        public LocalClientEditViewModel LocalClientEdit
        {
            get
            {
                if (localClientEdit == null)
                {
                    localClientEdit = new LocalClientEditViewModel();
                    localClientEdit.Port = 8000;
                    localClientEdit.AuthenticationToken = "abcdefgh";
                    localClientEdit.Delay = 2;
                    localClientEdit.Interval = 60;
                }

                return localClientEdit;
            }
        }

        private ImageViewModel image;
        public ImageViewModel Image
        {
            get
            {
                if (image == null)
                {
                    image = new ImageViewModel(new RemoteClient(Guid.NewGuid(), "Home", "http://home:8001", "xxxxx"));
                }

                return image;
            }
        }
    }
}
