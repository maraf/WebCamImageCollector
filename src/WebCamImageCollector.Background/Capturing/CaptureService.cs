using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WebCamImageCollector.Capturing
{
    internal class CaptureService
    {
        private const string stateFileName = "{653AC3C8-82D1-4474-A12B-13834A44CBD4}.tmp";

        private Timer timer;
        private DeviceInformation device;
        private MediaCapture mediaCapture;

        public TimeSpan Interval { get; private set; }
        public TimeSpan Delay { get; private set; }

        public bool IsRunning { get; private set; }

        public event Action<Exception> ExceptionHandler;

        public CaptureService(TimeSpan interval, TimeSpan delay, DeviceInformation preferredDevice = null)
        {
            Interval = interval;
            Delay = delay;
            device = preferredDevice;
        }

        public bool TryStartIfNotStopped()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(stateFileName))
            {
                Start();
                return true;
            }

            return false;
        }

        public void Start()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (!storage.FileExists(stateFileName))
                storage.CreateFile(stateFileName);

            if (timer == null)
                timer = new Timer(OnTimer, null, TimeSpan.Zero, Interval);
        }

        public void Stop()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(stateFileName))
                storage.DeleteFile(stateFileName);

            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        private void OnTimer(object state)
        {
            if (mediaCapture == null)
                TakePhoto();
        }

        private async void TakePhoto()
        {
            try
            {
                mediaCapture = new MediaCapture();

                if (device == null)
                {
                    await mediaCapture.InitializeAsync();
                }
                else
                {
                    MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings()
                    {
                        VideoDeviceId = device.Id,
                    };
                    await mediaCapture.InitializeAsync(settings);
                }

                if (Delay > TimeSpan.Zero)
                    await Task.Delay(Delay);

                string folderName = DateTime.Now.ToString("yyyy-MM-dd");
                StorageFolder folder = await KnownFolders.PicturesLibrary.FindFolderAsync(folderName);
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
                ExceptionHandler?.Invoke(ex);
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

        public IAsyncOperation<FileModel> FindLatestImageAsync()
        {
            return FindLatestImageInternalAsync().AsAsyncOperation();
        }

        private async Task<FileModel> FindLatestImageInternalAsync()
        {
            StorageFile latestFile = await FindLatestImage();
            if (latestFile == null)
                return null;

            return new FileModel(
                await latestFile.OpenSequentialReadAsync(),
                (long)(await latestFile.GetBasicPropertiesAsync()).Size,
                latestFile.DateCreated.DateTime
            );
        }

        private async Task<StorageFile> FindLatestImage()
        {
            IReadOnlyList<StorageFolder> folders = await KnownFolders.PicturesLibrary.GetFoldersAsync();
            DateTime dateTime;
            StorageFolder latestFolder = folders.OrderBy(f => f.Name).Where(f => DateTime.TryParse(f.Name, out dateTime)).LastOrDefault();
            if (latestFolder == null)
                return null;

            IReadOnlyList<StorageFile> files = await latestFolder.GetFilesAsync();
            StorageFile latestFile = files.OrderBy(f => f.DateCreated).LastOrDefault();

            return latestFile;
        }
    }
}
