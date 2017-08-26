using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
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
        public const int ImageMaxCount = 20;

        private readonly IClient client;
        private readonly DownloadImageCommand download;
        private readonly SaveImageCommand save;

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
        public ICommand ClearDownloaded { get; private set; }
        public ICommand Save => save;
        public ICommand Share { get; private set; }

        bool IClientStatusViewModel.IsRunning
        {
            get => IsRunning ?? false;
            set => IsRunning = value;
        }

        public ObservableCollection<ClientImageModel> Images { get; private set; }

        private ClientImageModel selectedImage;
        public ClientImageModel SelectedImage
        {
            get { return selectedImage; }
            set
            {
                if (selectedImage != value)
                {
                    selectedImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event Action DownloadFailed
        {
            add { download.Failed += value; }
            remove { download.Failed -= value; }
        }

        public event Action<ClientImageModel> DownloadCompleted
        {
            add { download.Completed += value; }
            remove { download.Completed -= value; }
        }

        public event Action SaveCompleted
        {
            add { save.Completed += value; }
            remove { save.Completed -= value; }
        }

        public event Action SaveFailed
        {
            add { save.Failed += value; }
            remove { save.Failed -= value; }
        }

        public ImageViewModel(IClient client)
        {
            Ensure.NotNull(client, "client");
            this.client = client;

            Quality = ImageQuality.Medium;
            Images = new ObservableCollection<ClientImageModel>();

            Start = new StartCommand(client, this);
            Stop = new StopCommand(client, this);
            CheckStatus = new CheckStatusCommand(client, this);

            download = new DownloadImageCommand(client, this);
            download.Completed += OnImageDownloaded;

            ClearDownloaded = new DelegateCommand(Images.Clear);
            save = new SaveImageCommand();
            Share = new ShareImageCommand(client);
        }

        private void OnImageDownloaded(ClientImageModel model)
        {
            ClientImageModel existing = Images.FirstOrDefault(i => i.Date == model.Date);
            if (existing != null)
                Images.Remove(existing);
            else if (Images.Count >= ImageMaxCount)
                Images.RemoveAt(0);

            Images.Add(model);
        }
    }
}
