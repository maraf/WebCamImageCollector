using Neptuo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using WebCamImageCollector.RemoteControl.ViewModels.Commands;
using System.Diagnostics;

namespace WebCamImageCollector.RemoteControl.Views
{
    public sealed partial class Image : NavigationPage, IMessagePage, IExceptionPage
    {
        private readonly ImageZoom zoom = new ImageZoom();

        public ImageViewModel ViewModel
        {
            get { return (ImageViewModel)DataContext; }
        }

        public Image()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await StatusBarProvider.TryExecuteAsync(async statusBar => await statusBar.HideAsync());

            Guid key = (Guid)e.Parameter;

            ClientRepository repository = new ClientRepository();
            IClient client = repository.FindRemote(key);
            if (client == null)
                client = repository.FindLocal();

            if (client == null)
                throw Ensure.Exception.ArgumentOutOfRange("parameter", "Unnable to find a client with key '{0}'.", key);

            DataContext = new ImageViewModel(client);

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.DownloadCompleted += m =>
            {
                ShowError(string.Empty);
                ImageList.SelectedIndex = ViewModel.Images.Count - 1;
            };
            ViewModel.DownloadFailed += type =>
            {
                switch (type)
                {
                    case DownloadImageCommand.FailType.ClientError:
                        ShowError("Downloading failed");
                        break;
                    case DownloadImageCommand.FailType.Cancelled:
                        ShowError("Downloading cancelled");
                        break;
                    default:
                        ShowError("Unknown error while downloading the image");
                        Debug.Fail($"Missing switch for {type}.");
                        break;
                }
            };
            ViewModel.SaveCompleted += () => ShowInfo("Saved");
            ViewModel.SaveFailed += () => ShowError("Something went wrong saving the image");
            ViewModel.CheckStatus.Execute(null);
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            await StatusBarProvider.TryExecuteAsync(async statusBar => await statusBar.ShowAsync());
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsRunning))
            {
                ShowStatusMessage();
            }
            else if (e.PropertyName == nameof(ViewModel.IsStatusLoading))
            {
                if (ViewModel.IsStatusLoading)
                    ShowInfo("Loading status...");
                else
                    ShowStatusMessage();
            }
            else if (e.PropertyName == nameof(ViewModel.IsDownloading))
            {
                if (ViewModel.IsDownloading)
                    ShowInfo("Downloading image...");
            }
            else if (e.PropertyName == nameof(ViewModel.SelectedImage))
            {
                if (ViewModel.SelectedImage != null)
                {
                    FlipViewItem item = (FlipViewItem)ImageList.ContainerFromItem(ViewModel.SelectedImage);
                    ScrollViewer itemView = (ScrollViewer)item.ContentTemplateRoot;

                    zoom.Apply(itemView, ViewModel.SelectedImage);
                }
            }
        }

        private void ShowStatusMessage()
        {
            if (ViewModel.IsRunning == null)
                ShowError("Client is not responding");
            else if (ViewModel.IsRunning.Value)
                ShowInfo("Client is running");
            else
                ShowInfo("Client is not running");
        }

        private void ContentPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.OriginalSource == ContentPanel || e.OriginalSource is Windows.UI.Xaml.Controls.Image)
                MessagePanel.Visibility = MessagePanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ShowDate(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string text)
        {
            InfoMessage.Visibility = Visibility.Collapsed;
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage.Text = text;

            MessagePanel.Visibility = Visibility.Visible;
        }

        public void ShowInfo(string text)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
            InfoMessage.Visibility = Visibility.Visible;
            InfoMessage.Text = text;

            MessagePanel.Visibility = Visibility.Visible;
        }

        public bool TryProcess(Exception e)
        {
            if (e is ClientException)
                ShowInfo(string.Empty);

            return false;
        }

        private void ImageScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer view = (ScrollViewer)sender;
            zoom.Save(view);
        }
    }
}
