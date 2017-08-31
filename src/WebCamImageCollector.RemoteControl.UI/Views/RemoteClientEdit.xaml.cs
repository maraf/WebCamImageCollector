using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebCamImageCollector.RemoteControl.ViewModels;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace WebCamImageCollector.RemoteControl.Views
{
    public sealed partial class RemoteClientEdit : EditPage
    {
        public RemoteClientEdit()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Guid? key = (Guid?)e.Parameter;
            if (key == null)
            {
                DataContext = new RemoteClientEditViewModel();
                IsNewRecord = true;
            }
            else
            {
                DataContext = new RemoteClientEditViewModel(key.Value);
                IsNewRecord = false;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Name.Focus(FocusState.Keyboard);
            SelectText(Name);
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            CoreVirtualKeyStates ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
            {
                if (e.OriginalKey == VirtualKey.S)
                    Save.Command?.Execute(null);
                else if (e.OriginalKey == VirtualKey.D)
                    Delete.Command?.Execute(null);
            }
        }

        private void OnTextBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (IsTouchMode && e.Key == VirtualKey.Enter)
            {
                TextBox target = null;

                if (sender == Name)
                    target = Url;
                else if (sender == Url)
                    target = AuthenticationToken;

                if (target != null)
                {
                    target.Focus(FocusState.Keyboard);
                    SelectText(target);
                }
                else
                {
                    Save.Command?.Execute(null);
                }
            }
        }
    }
}
