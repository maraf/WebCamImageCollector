using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
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
    public partial class ImageViewModel : ObservableObject, IClientStatusViewModel, CheckStatusCommand.IViewModel, DownloadImageCommand.IViewModel
    {
        private readonly IClient client;
        private readonly DownloadImageCommand download;

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

        private bool isDownlading;
        public bool IsDownloading
        {
            get { return isDownlading; }
            set
            {
                if (isDownlading != value)
                {
                    isDownlading = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand Start { get; private set; }
        public ICommand Stop { get; private set; }
        public ICommand CheckStatus { get; private set; }
        public ICommand Download => download;

        bool IClientStatusViewModel.IsRunning
        {
            get => IsRunning ?? false;
            set => IsRunning = value;
        }

        public ObservableCollection<ClientImageModel> Images { get; private set; }

        public event Action DownloadFailed
        {
            add { download.Failed += value; }
            remove { download.Failed -= value; }
        }

        public ImageViewModel(IClient client)
        {
            Ensure.NotNull(client, "client");
            this.client = client;

            Quality = ImageQuality.Medium;

            Start = new StartCommand(client, this);
            Stop = new StopCommand(client, this);
            CheckStatus = new CheckStatusCommand(client, this);

            download = new DownloadImageCommand(client, this);
            download.Completed += OnImageDownloaded;

            Images = new ObservableCollection<ClientImageModel>();
        }

        private void OnImageDownloaded(ClientImageModel model)
        {
            Images.Add(model);
        }
    }
}
