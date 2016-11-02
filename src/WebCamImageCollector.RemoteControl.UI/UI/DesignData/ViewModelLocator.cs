using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.UI.DesignData
{
    internal class ViewModelLocator
    {
        private MainViewModel mainViewModel;

        public MainViewModel MainViewModel
        {
            get
            {
                if (mainViewModel == null)
                {
                    mainViewModel = new MainViewModel(new MainViewModelService(), null);
                    mainViewModel.LocalClient = new ClientViewModel()
                    {
                        Name = "Local",
                        Url = "http://localhost:9000"
                    };
                    mainViewModel.RemoteClients.Add(new ClientViewModel()
                    {
                        Name = "Raspberry Pi2",
                        Url = "http://rpi2athome:9001"
                    });
                    mainViewModel.RemoteClients.Add(new ClientViewModel()
                    {
                        Name = "Public web cam",
                        Url = "http://publis-web-cam.at:9002"
                    });

                    //mainViewModel.RemoteClientEdit = new RemoteClientEditViewModel(new RemoteClientEditViewModelService())
                    //{
                    //    Name = "Raspberry Pi2",
                    //    Url = "http://rpi2athome:9001",
                    //    AuthenticationToken = "{FFA23D94-A341-4CBB-B40F-8D6D3B5C2408}"
                    //};
                    //mainViewModel.LocalClientEdit = new LocalClientEditViewModel()
                    //{
                    //    Port = 9010,
                    //    AuthenticationToken = "{FFA23D94-A341-4CBB-B40F-8D6D3B5C2408}",
                    //    IntervalSeconds = 60,
                    //    DelaySeconds = 5
                    //};
                }
                return mainViewModel;
            }
        }

        private class MainViewModelService : MainViewModel.IService
        {
            public ClientViewModel CreateLocal(int port, string authenticationToken, int interval, int delay)
            {
                throw new NotImplementedException();
            }

            public ClientViewModel CreateRemote(string name, string url, string authenticationToken)
            {
                throw new NotImplementedException();
            }
        }

        private class RemoteClientEditViewModelService : RemoteClientEditViewModel.IService
        {
            public void Close()
            {
                throw new NotImplementedException();
            }

            public void Delete()
            {
                throw new NotImplementedException();
            }

            public void Save(string name, string url, string authenticationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
