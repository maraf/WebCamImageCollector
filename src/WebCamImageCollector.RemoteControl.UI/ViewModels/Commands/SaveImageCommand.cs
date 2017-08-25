using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using System.Threading;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.IO;
using Windows.Storage.Provider;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class SaveImageCommand : AsyncCommand<ClientImageModel>
    {
        public event Action Completed;
        public event Action Failed;

        protected override bool CanExecuteOverride(ClientImageModel parameter)
        {
            return true;
        }

        protected override async Task ExecuteAsync(ClientImageModel parameter, CancellationToken cancellationToken)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = parameter.Date.ToString("yyyy-MM-dd HH:mm:ss");

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (MemoryStream buffer = new MemoryStream())
                {
                    parameter.Stream.Position = 0;
                    parameter.Stream.CopyTo(buffer);

                    await FileIO.WriteBytesAsync(file, buffer.ToArray());
                }

                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                    Completed?.Invoke();
                else
                    Failed?.Invoke();
            }
        }
    }
}
