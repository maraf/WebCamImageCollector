using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebCamImageCollector.RemoteControl.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        private void OnTextBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
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
