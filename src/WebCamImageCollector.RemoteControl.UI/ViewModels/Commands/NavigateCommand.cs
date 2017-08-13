using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class NavigateCommand : Command
    {
        private readonly Type pageType;
        private readonly object parameter;

        public NavigateCommand(Type pageType)
        {
            Ensure.NotNull(pageType, "pageType");
            this.pageType = pageType;
        }

        public NavigateCommand(Type pageType, object parameter)
        {
            Ensure.NotNull(pageType, "pageType");
            Ensure.NotNull(parameter, "parameter");
            this.pageType = pageType;
            this.parameter = parameter;
        }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame != null)
                frame.Navigate(pageType, parameter);
        }
    }
}
