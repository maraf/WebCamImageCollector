using Neptuo;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Views;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class SaveLocalCommand : NavigateCommand
    {
        private readonly LocalClientEditViewModel viewModel;

        public SaveLocalCommand(LocalClientEditViewModel viewModel) 
            : base(typeof(Overview))
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;

            viewModel.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override bool CanExecute()
        {
            return viewModel.Port > 0 && !string.IsNullOrEmpty(viewModel.AuthenticationToken) && viewModel.Interval > 5;
        }

        public override void Execute()
        {
            ClientRepository repository = new ClientRepository();
            repository.CreateOrReplaceLocal(viewModel.Port, viewModel.AuthenticationToken, viewModel.Interval, viewModel.Delay);

            base.Execute();
        }
    }
}
