using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace WebCamImageCollector.Background
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private Timer _timer;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            if (_timer == null)
                _timer = new Timer(OnPeriodicPhotoTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            WebServer server = new WebServer();
            server.Start().Wait();
        }

        private void OnPeriodicPhotoTimer(object state)
        {
            TakePhoto();
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => takePhoto_Click(null, null));
        }

        private async void TakePhoto()
        {
            MediaCapture mediaCapture = null;
            try
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                StorageFolder folder = await KnownFolders.PicturesLibrary.GetFolderAsync(System.DateTime.Now.ToString("yyyy-MM-dd"));
                if (folder == null)
                    folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(System.DateTime.Now.ToString("yyyy-MM-dd"));

                StorageFile photoFile = await folder.CreateFileAsync($"{System.DateTime.Now.ToString("HH-mm-ss")}.jpg", CreationCollisionOption.GenerateUniqueName);

                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
            }
            catch (Exception ex)
            {
                deferral.Complete();
            }
            finally
            {
                if (mediaCapture != null)
                {
                    //await mediaCapture.StopPreviewAsync();
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }
            }
        }
    }
}
