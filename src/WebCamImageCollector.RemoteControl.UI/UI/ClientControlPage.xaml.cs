using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI;
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
    public sealed partial class ClientControlPage : Page
    {
        private IClient client;

        public ClientControlPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            client = ServiceProvider.Clients.Find((Guid)e.Parameter);
            if (client == null)
            {
                DisableButtons();
                ShowMessage("Missing client.");
                return;
            }

            DisableButtons();
            UpdateState();
        }

        public void ShowMessage(string message, bool isError = false)
        {
            tblMessage.Foreground = new SolidColorBrush(isError ? Colors.Red : Colors.White);
            tblMessage.Text = message;
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
            await HandleErrorAsync(client.IsRunningAsync, status =>
            {
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

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Starting server...");
            await HandleErrorAsync(client.StartAsync, () =>
            {
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
            });
        }

        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Stoping server...");
            await HandleErrorAsync(client.StopAsync, () =>
            {
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
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
            await HandleErrorAsync(client.DownloadLatest, model =>
            {
                downloadModel = model;
                imgBackground.Source = model.Image;
                ClearMessage();
            });
        }

        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            imgBackground.Source = null;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
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
                    ShowMessage("Save completed...");
                else
                    ShowMessage("Something went wrong during save operation.");
            }
        }
    }
}
