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
using System.Threading.Tasks;
using Windows.Foundation;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace WebCamImageCollector.Background
{
    public sealed class StartupTask : IBackgroundTask, ICaptureService
    {
        private BackgroundTaskDeferral deferral;
        private WebServer server;
        private Timer timer;

        public bool IsRunning
        {
            get { return timer != null; }
        }

        public void Start()
        {
            if (timer == null)
                timer = new Timer(OnPeriodicPhotoTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            server = new WebServer(this, "abcdef");
            await server.StartAsync(8000);
        }

        private void OnPeriodicPhotoTimer(object state)
        {
            TakePhoto();
        }

        private async void TakePhoto()
        {
            MediaCapture mediaCapture = null;
            try
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                string folderName = DateTime.Now.ToString("yyyy-MM-dd");
                StorageFolder folder = await FindFolderAsync(KnownFolders.PicturesLibrary, folderName);
                if (folder == null)
                    folder = await KnownFolders.PicturesLibrary.CreateFolderAsync(folderName);

                StorageFile photoFile = await folder.CreateFileAsync(
                    $"{DateTime.Now.ToString("HH-mm-ss")}.jpg", 
                    CreationCollisionOption.GenerateUniqueName
                );

                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
            }
            catch (Exception ex)
            {
                deferral.Complete();
                server.Dispose();
            }
            finally
            {
                if (mediaCapture != null)
                {
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }
            }
        }

        private async Task<StorageFolder> FindFolderAsync(StorageFolder parent, string folderName)
        {
            try
            {
                StorageFolder item = await parent.TryGetItemAsync(folderName) as StorageFolder;
                return item;
            }
            catch (Exception)
            {
                // Should never get here 
                return null;
            }
        }
    }
}
