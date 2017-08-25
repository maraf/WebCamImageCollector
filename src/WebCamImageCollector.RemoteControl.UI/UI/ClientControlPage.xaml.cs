using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.Views;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WebCamImageCollector.RemoteControl.UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClientControlPage : NavigationPage, IMessagePage
    {
        private IClient client;
        private ImageQuality quality;
        private Type sourcePageType;

        public ClientControlPage()
        {
            InitializeComponent();
        }

        private Task TryUseStatusBar(Func<StatusBar, Task> handler)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                return handler(statusBar);
            }

            return Task.CompletedTask;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            sourcePageType = Frame.BackStack.LastOrDefault().SourcePageType;

            await TryUseStatusBar(async bar => await bar.HideAsync());

            client = ServiceProvider.Clients.Find((Guid)e.Parameter);
            if (client == null)
            {
                DisableButtons();
                ShowMessage("Missing client.");
                return;
            }

            DisableButtons();
            UpdateState();
            UpdateQualityButtons();

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += OnDataTransferRequested;
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            await TryUseStatusBar(async bar => await bar.ShowAsync());
        }

        private void OnDataTransferRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (downloadModel == null)
                return;

            DataRequest request = args.Request;
            DataRequestDeferral deferral = request.GetDeferral();
            try
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    downloadModel.Stream.Position = 0;
                    downloadModel.Stream.CopyTo(buffer);
                    buffer.Position = 0;

                    request.Data.Properties.Title = downloadModel.Date.ToString("yyyy-MM-dd HH:mm:ss");
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

        public void ShowMessage(string message, bool isError = false)
        {
            tblMessage.Foreground = new SolidColorBrush(isError ? Colors.Red : Colors.White);
            tblMessage.Text = message;
        }

        public void ShowInfo(string text)
        {
            ShowMessage(text, false);
        }

        public void ShowError(string text)
        {
            ShowMessage(text, true);
        }

        private void ClearMessage()
        {
            tblMessage.Text = String.Empty;
        }

        private void OnNetworkError(HttpRequestException e)
        {
            DisableButtons();
            ShowMessage(e.Message, true);
        }

        private void DisableButtons()
        {
            btnDownload.IsEnabled = false;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = false;
        }

        private async Task UpdateState()
        {
            ShowMessage("Checking status...");
            await HandleErrorAsync(() => client.IsRunningAsync(), status =>
            {
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = true;
                btnDownload.IsEnabled = true;

                if (status.Running)
                {
                    btnStart.Visibility = Visibility.Collapsed;
                    btnStop.Visibility = Visibility.Visible;
                }
                else
                {
                    if (!String.IsNullOrEmpty(status.LastError))
                        ShowMessage(status.LastError);

                    btnStart.Visibility = Visibility.Visible;
                    btnStop.Visibility = Visibility.Collapsed;
                }
            });
        }

        private async Task HandleErrorAsync(Func<Task> execute, Action done)
        {
            try
            {
                await execute();
                ClearMessage();
                done();
            }
            catch (ClientException)
            {
                ShowMessage("Server not responded correctly.", true);
            }
            catch (HttpRequestException requestException)
            {
                OnNetworkError(requestException);
            }
            catch (Exception e)
            {
                string message = e.ToString();
                ShowMessage(message.Substring(0, Math.Min(message.Length, 20)));
            }
        }

        private async Task HandleErrorAsync<T>(Func<Task<T>> execute, Action<T> done)
        {
            try
            {
                T result = await execute();
                ClearMessage();
                done(result);
            }
            catch (ClientNotAvailableException)
            {
                ShowMessage("Image download failed.", true);
            }
            catch (ClientException)
            {
                ShowMessage("Server not responded correctly.", true);
            }
            catch (HttpRequestException requestException)
            {
                OnNetworkError(requestException);
            }
            catch (Exception e)
            {
                string message = e.Message;
                if (message.Length > 30)
                    message = message.Substring(0, Math.Min(message.Length, 30));

                ShowMessage(message, true);
            }
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Starting server...");
            await HandleErrorAsync(() => client.StartAsync(), () =>
            {
                btnStart.Visibility = Visibility.Collapsed;
                btnStop.Visibility = Visibility.Visible;
            });
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Stoping server...");
            await HandleErrorAsync(() => client.StopAsync(), () =>
            {
                btnStart.Visibility = Visibility.Visible;
                btnStop.Visibility = Visibility.Collapsed;
            });
        }

        private async void btnStatus_Click(object sender, RoutedEventArgs e)
        {
            ClearMessage();
            await UpdateState();
        }

        private ClientImageModel downloadModel;

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Downloading image...");
            await HandleErrorAsync(() => client.DownloadLatest(quality), model =>
            {
                downloadModel = model;
                imgBackground.Source = model.Image;
                ClearMessage();

                string date = model.Date.ToString("HH:mm:ss");
                if (model.Date.Date != DateTime.Today)
                    date += model.Date.ToString(" dd.MM.yyyy");

                ShowMessage(date);
            });
        }

        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            imgBackground.Source = null;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            if (sourcePageType == typeof(Overview))
                Frame.Navigate(typeof(Overview));
            else
                Frame.Navigate(typeof(MainPage));
        }

        private async void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = downloadModel.Date.ToString("yyyy-MM-dd HH:mm:ss");

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (MemoryStream buffer = new MemoryStream())
                {
                    downloadModel.Stream.Position = 0;
                    downloadModel.Stream.CopyTo(buffer);

                    await FileIO.WriteBytesAsync(file, buffer.ToArray());
                }

                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                    ShowMessage("Saved.");
                else
                    ShowMessage("Something went wrong saving the image.");
            }
        }

        private void mfiQualityFull_Click(object sender, RoutedEventArgs e)
        {
            quality = ImageQuality.Full;
            UpdateQualityButtons();
        }

        private void mfiQualityMedium_Click(object sender, RoutedEventArgs e)
        {
            quality = ImageQuality.Medium;
            UpdateQualityButtons();
        }

        private void mfiQualityThumbnail_Click(object sender, RoutedEventArgs e)
        {
            quality = ImageQuality.Thumbnail;
            UpdateQualityButtons();
        }

        private void UpdateQualityButtons()
        {
            mfiQualityFull.IsChecked = false;
            mfiQualityMedium.IsChecked = false;
            mfiQualityThumbnail.IsChecked = false;
            switch (quality)
            {
                case ImageQuality.Full:
                    mfiQualityFull.IsChecked = true;
                    abbQuality.Icon = new SymbolIcon(Symbol.FourBars);
                    break;
                case ImageQuality.Medium:
                    mfiQualityMedium.IsChecked = true;
                    abbQuality.Icon = new SymbolIcon(Symbol.ThreeBars);
                    break;
                case ImageQuality.Thumbnail:
                    mfiQualityThumbnail.IsChecked = true;
                    abbQuality.Icon = new SymbolIcon(Symbol.OneBar);
                    break;
            }
        }

        private void abbShare_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}
