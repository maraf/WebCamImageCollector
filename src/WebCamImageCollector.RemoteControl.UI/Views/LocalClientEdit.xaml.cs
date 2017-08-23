using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WebCamImageCollector.RemoteControl.Services;
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
    public sealed partial class LocalClientEdit : EditPage
    {
        public LocalClientEdit()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ClientRepository repository = new ClientRepository();
            LocalClient client = repository.FindLocal();

            IsNewRecord = client == null;
            DataContext = new LocalClientEditViewModel(client);

            Port.Focus(FocusState.Keyboard);
            SelectText(Port);
        }

        private void OnTextBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                TextBox target = null;

                if (sender == Port)
                    target = AuthenticationToken;
                else if (sender == AuthenticationToken)
                    target = Interval;
                else if (sender == Interval)
                    target = Delay;

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
