using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.UI
{
    public class RemoteClientEditViewModel : ObservableObject
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string authenticationToken;
        public string AuthenticationToken
        {
            get { return authenticationToken; }
            set
            {
                if (authenticationToken != value)
                {
                    authenticationToken = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
