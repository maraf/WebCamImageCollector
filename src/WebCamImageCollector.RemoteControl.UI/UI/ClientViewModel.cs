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
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
