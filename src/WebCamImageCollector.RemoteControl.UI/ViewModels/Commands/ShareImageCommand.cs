using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class ShareImageCommand : Command<ClientImageModel>
    {
        private readonly IClient client;
        private ClientImageModel parameter;

        public ShareImageCommand(IClient client)
        {
            Ensure.NotNull(client, "client");
            this.client = client;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += OnDataTransferRequested;
        }

        private void OnDataTransferRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (parameter == null)
                return;

            DataRequest request = args.Request;
            DataRequestDeferral deferral = request.GetDeferral();
            try
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    parameter.Stream.Position = 0;
                    parameter.Stream.CopyTo(buffer);
                    buffer.Position = 0;

                    request.Data.Properties.Title = parameter.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    request.Data.Properties.Description = client.Name;
                    request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromStream(buffer.AsRandomAccessStream());
                    request.Data.SetBitmap(RandomAccessStreamReference.CreateFromStream(buffer.AsRandomAccessStream()));
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        public override bool CanExecute(ClientImageModel parameter)
        {
            return DataTransferManager.IsSupported();
        }

        public override void Execute(ClientImageModel parameter)
        {
            this.parameter = parameter;
            DataTransferManager.ShowShareUI();
        }
    }
}
