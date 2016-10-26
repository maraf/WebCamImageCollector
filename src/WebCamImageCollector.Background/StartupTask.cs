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
using System.IO;
using Windows.Storage.Search;
using System.IO.IsolatedStorage;
using Windows.Devices.Enumeration;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace WebCamImageCollector.Background
{
    public sealed class StartupTask : IBackgroundTask, ICaptureService
    {
        private const string stateFileName = "{653AC3C8-82D1-4474-A12B-13834A44CBD4}.tmp";

        private BackgroundTaskDeferral deferral;
        private WebServer server;
        private Timer timer;
        private DeviceInformation device;
        private TimeSpan interval;
        private TimeSpan? delay;

        #region ICaptureService

        public bool IsRunning
        {
            get { return timer != null; }
        }

        public void Start()
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (!storage.FileExists(stateFileName))
                storage.CreateFile(stateFileName);

            if (timer == null)
                timer = new Timer(OnPeriodicPhotoTimer, null, TimeSpan.Zero, interval);
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
                (long)(await latestFile.GetBasicPropertiesAsync()).Size
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

        #endregion

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            int port = 8000;
            string authenticationToken = "{3FFF8234-F0B4-4DEB-AB91-75C98ECE550D}";
            interval = TimeSpan.FromMinutes(1);

            ApplicationTriggerDetails triggerDetails = taskInstance.TriggerDetails as ApplicationTriggerDetails;
            if (triggerDetails != null)
            {
                object portRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Port", out portRaw) && portRaw != null)
                    port = Int32.Parse(portRaw.ToString());

                object authTokenRaw = null;
                if (triggerDetails.Arguments.TryGetValue("AuthenticationToken", out authTokenRaw) && authTokenRaw != null)
                    authenticationToken = authTokenRaw.ToString();

                object delayRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Delay", out delayRaw) && delayRaw != null)
                    delay = TimeSpan.FromSeconds(Int32.Parse(delayRaw.ToString()));

                object intervalRaw = null;
                if (triggerDetails.Arguments.TryGetValue("Interval", out intervalRaw) && intervalRaw != null)
                {
                    TimeSpan interval = TimeSpan.FromSeconds(Int32.Parse(intervalRaw.ToString()));
                    if (interval > TimeSpan.FromSeconds(10) && interval < TimeSpan.FromMinutes(5))
                        this.interval = interval;
                }
            }

            server = new WebServer(this, authenticationToken);
            await server.StartAsync(port);

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            device = devices.Where(d => d.EnclosureLocation != null).FirstOrDefault(d => d.EnclosureLocation.Panel == Panel.Back);
            if (device == null)
                device = devices.FirstOrDefault();

            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            if (storage.FileExists(stateFileName))
                Start();
        }

        private void OnPeriodicPhotoTimer(object state)
        {
            if (mediaCapture == null)
                TakePhoto();
        }

        private MediaCapture mediaCapture = null;

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

                if (delay != null)
                    await Task.Delay(delay.Value);
                
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
                //server.Dispose();
                //deferral.Complete();
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
    }
}
