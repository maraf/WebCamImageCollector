using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public abstract class DeleteCommandBase : NavigateCommand
    {
        private readonly string content;

        public DeleteCommandBase(Type pageType, string content) 
            : base(pageType)
        {
            this.content = content;
        }

        public override async void Execute()
        {
            MessageDialog dialog = new MessageDialog(content);

            UICommand ok = new UICommand("&Yes");
            dialog.Commands.Add(ok);
            dialog.Commands.Add(new UICommand("&No"));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            IUICommand command = await dialog.ShowAsync();
            if (command == ok && ExecuteOverride())
                base.Execute();
        }

        protected abstract bool ExecuteOverride();
    }
}
