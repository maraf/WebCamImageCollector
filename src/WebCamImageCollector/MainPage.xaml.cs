using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebCamImageCollector
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCapture;
        private StorageFile photoFile;
        private bool isPreviewing;
        private bool isRecording;

        #region HELPER_FUNCTIONS

        enum Action
        {
            Enable,
            Disable
        }

        /// <summary>
        /// Helper function to enable or disable Initialization buttons
        /// </summary>
        /// <param name="action">enum Action</param>
        private void SetInitButtonVisibility(Action action)
        {
            if (action == Action.Enable)
            {
                video_init.IsEnabled = true;
            }
            else
            {
                video_init.IsEnabled = false;
            }
        }

        /// <summary>
        /// Helper function to enable or disable video related buttons (TakePhoto, Start Video Record)
        /// </summary>
        /// <param name="action">enum Action</param>
        private void SetVideoButtonVisibility(Action action)
        {
            if (action == Action.Enable)
            {
                takePhoto.IsEnabled = true;
                takePeriodicPhoto.IsEnabled = true;
                takePhoto.Visibility = Visibility.Visible;
                takePeriodicPhoto.Visibility = Visibility.Visible;
            }
            else
            {
                takePhoto.IsEnabled = false;
                takePeriodicPhoto.IsEnabled = false;
                takePhoto.Visibility = Visibility.Collapsed;
                takePeriodicPhoto.Visibility = Visibility.Collapsed;
            }
        }
        
        #endregion
        public MainPage()
        {
            this.InitializeComponent();

            SetInitButtonVisibility(Action.Enable);
            SetVideoButtonVisibility(Action.Disable);

            isRecording = false;
            isPreviewing = false;
        }

        private async void Cleanup()
        {
            if (mediaCapture != null)
            {
                // Cleanup MediaCapture object
                if (isPreviewing)
                {
                    await mediaCapture.StopPreviewAsync();
                    captureImage.Source = null;
                    isPreviewing = false;
                }
                if (isRecording)
                {
                    await mediaCapture.StopRecordAsync();
                    isRecording = false;
                }
                mediaCapture.Dispose();
                mediaCapture = null;
            }
            SetInitButtonVisibility(Action.Enable);
        }

        /// <summary>
        /// 'Initialize Audio and Video' button action function
        /// Dispose existing MediaCapture object and set it up for audio and video
        /// Enable or disable appropriate buttons
        /// - DISABLE 'Initialize Audio and Video' 
        /// - DISABLE 'Start Audio Record'
        /// - ENABLE 'Initialize Audio Only'
        /// - ENABLE 'Start Video Record'
        /// - ENABLE 'Take Photo'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void initVideo_Click(object sender, RoutedEventArgs e)
        {
            // Disable all buttons until initialization completes

            SetInitButtonVisibility(Action.Disable);
            SetVideoButtonVisibility(Action.Disable);

            try
            {
                if (mediaCapture != null)
                {
                    // Cleanup MediaCapture object
                    if (isPreviewing)
                    {
                        await mediaCapture.StopPreviewAsync();
                        captureImage.Source = null;
                        isPreviewing = false;
                    }
                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        isRecording = false;
                    }
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                status.Text = "Initializing camera to capture audio and video...";
                // Use default initialization
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                // Set callbacks for failure and recording limit exceeded
                status.Text = "Device successfully initialized for video recording!";
                mediaCapture.Failed += mediaCapture_Failed;

                // Start Preview                
                previewElement.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
                status.Text = "Camera preview succeeded";

                // Enable buttons for video and photo capture
                SetVideoButtonVisibility(Action.Enable);
            }
            catch (Exception ex)
            {
                status.Text = "Unable to initialize camera for audio/video mode: " + ex.Message;
            }
        }

        private void cleanup_Click(object sender, RoutedEventArgs e)
        {
            SetInitButtonVisibility(Action.Disable);
            SetVideoButtonVisibility(Action.Disable);
            Cleanup();
        }
        
        /// <summary>
        /// 'Take Photo' button click action function
        /// Capture image to a file in the default account photos folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void takePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                takePhoto.IsEnabled = false;
                takePeriodicPhoto.IsEnabled = false;
                captureImage.Source = null;

                photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync($"{System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.jpg", CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
                takePhoto.IsEnabled = true;
                takePeriodicPhoto.IsEnabled = true;
                status.Text = "Take Photo succeeded: " + photoFile.Path;

                IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(photoStream);
                captureImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
                Cleanup();
            }
            finally
            {
                takePhoto.IsEnabled = true;
                takePeriodicPhoto.IsEnabled = true;
            }
        }
        
        /// <summary>
        /// Callback function for any failures in MediaCapture operations
        /// </summary>
        /// <param name="currentCaptureObject"></param>
        /// <param name="currentFailure"></param>
        private async void mediaCapture_Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    status.Text = "MediaCaptureFailed: " + currentFailure.Message;

                    if (isRecording)
                    {
                        await mediaCapture.StopRecordAsync();
                        status.Text += "\n Recording Stopped";
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    SetInitButtonVisibility(Action.Disable);
                    SetVideoButtonVisibility(Action.Disable);
                    status.Text += "\nCheck if camera is diconnected. Try re-launching the app";
                }
            });
        }

        private Timer _timer;

        private void takePeriodicPhoto_Click(object sender, RoutedEventArgs e)
        {
            if(_timer == null)
            {
                _timer = new Timer(OnPeriodicPhotoTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                takePeriodicPhoto.Content = "Stop";
            }
            else
            {
                _timer.Dispose();
                _timer = null;
                takePeriodicPhoto.Content = "Take Photo every 1min";
            }
        }

        private async void OnPeriodicPhotoTimer(object state)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => takePhoto_Click(null, null));
        }
    }
}
