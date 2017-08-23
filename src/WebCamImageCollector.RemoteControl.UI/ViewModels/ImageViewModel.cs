using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.ViewModels
{
    public class ImageViewModel : ObservableObject
    {
        private readonly IClient client;

        private ImageQuality quality;
        public ImageQuality Quality
        {
            get { return quality; }
            set
            {
                if (quality != value)
                {
                    quality = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ImageViewModel(IClient client)
        {
            Ensure.NotNull(client, "client");
            this.client = client;

            Quality = ImageQuality.Medium;
        }   
    }
}
