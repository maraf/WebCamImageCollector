using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace WebCamImageCollector.RemoteControl.Views
{
    public class NavigationPage : Page
    {
        private SystemNavigationManager navigationManager;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            navigationManager = SystemNavigationManager.GetForCurrentView();
            navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            navigationManager.BackRequested += OnBackRequested;

            PointerPressed += OnPointerPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (navigationManager != null)
            {
                navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                navigationManager.BackRequested -= OnBackRequested;
            }

            PointerPressed -= OnPointerPressed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.BackStackDepth > 0)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(Overview));
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint point = e.GetCurrentPoint(this);
                if (point.Properties.IsXButton1Pressed && Frame.BackStackDepth > 0)
                    Frame.GoBack();
            }
        }
    }
}
