using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.RemoteControl.UI
{
    public class ClientViewModel : ObservableObject
    {
        private Guid key;
        public Guid Key
        {
            get { return key; }
            set
            {
                if (key != value)
                {
                    key = value;
                    RaisePropertyChanged();
                }
            }
        }

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
    }
}
