using Neptuo;
using Neptuo.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.ViewModels.Commands;

namespace WebCamImageCollector.RemoteControl.ViewModels
{
    public partial class ImageViewModel : ObservableObject, IClientStatusViewModel, CheckStatusCommand.IViewModel
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

        private bool? isRunning;
        public bool? IsRunning
        {
            get { return isRunning; }
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isStatusLoading;
        public bool IsStatusLoading
        {
            get { return isStatusLoading; }
            set
            {
                if (isStatusLoading != value)
                {
                    isStatusLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand Start { get; private set; }
        public ICommand Stop { get; private set; }
        public ICommand CheckStatus { get; private set; }

        bool IClientStatusViewModel.IsRunning
        {
            get => IsRunning ?? false;
            set => IsRunning = value;
        }

        public ImageViewModel(IClient client)
        {
            Ensure.NotNull(client, "client");
            this.client = client;

            Quality = ImageQuality.Medium;

            Start = new StartCommand(client, this);
            Stop = new StopCommand(client, this);
            CheckStatus = new CheckStatusCommand(client, this);
        }
    }
}
