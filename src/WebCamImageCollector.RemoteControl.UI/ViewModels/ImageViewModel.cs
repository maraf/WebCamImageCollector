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
        private readonly IMessageService messages;

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

                if (value == null)
                    messages.ShowError("Client is not responding");
                else if (value.Value)
                    messages.ShowInfo("Client is running");
                else
                    messages.ShowInfo("Client is not running");
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

                    if (value)
                        messages.ShowInfo("Loading status...");
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

        public ImageViewModel(IClient client, IMessageService messages)
        {
            Ensure.NotNull(client, "client");
            Ensure.NotNull(messages, "messages");
            this.client = client;
            this.messages = messages;

            Quality = ImageQuality.Medium;

            Start = new StartCommand(client, this);
            Stop = new StopCommand(client, this);
            CheckStatus = new CheckStatusCommand(client, this);

            CheckStatus.Execute(null);
        }
    }
}
