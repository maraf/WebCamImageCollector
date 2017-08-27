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
using Windows.Storage;
using Windows.Storage.Provider;
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

        private async void OnDataTransferRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (parameter == null)
                return;

            DataRequest request = args.Request;
            DataRequestDeferral deferral = request.GetDeferral();
            StorageFile file = null;

            try
            {
                file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync($"SharedImage.jpg", CreationCollisionOption.GenerateUniqueName);

                CachedFileManager.DeferUpdates(file);

                using (MemoryStream buffer = new MemoryStream())
                {
                    parameter.Stream.Position = 0;
                    parameter.Stream.CopyTo(buffer);

                    await FileIO.WriteBytesAsync(file, buffer.ToArray());
                }

                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    request.Data.Properties.Title = parameter.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    request.Data.Properties.Description = client.Name;
                    request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(file);
                    request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
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
