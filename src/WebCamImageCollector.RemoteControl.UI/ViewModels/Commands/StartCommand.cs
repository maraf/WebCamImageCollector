using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class StartCommand : AsyncCommand
    {
        private readonly IClient client;
        private readonly IClientStatusViewModel viewModel;

        public StartCommand(IClient client, IClientStatusViewModel viewModel)
        {
            Ensure.NotNull(client, "client");
            Ensure.NotNull(viewModel, "viewModel");
            this.client = client;
            this.viewModel = viewModel;
        }

        protected override bool CanExecuteOverride()
        {
            return true;
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            viewModel.IsRunning = await client.StartAsync(cancellationToken);
        }
    }
}
