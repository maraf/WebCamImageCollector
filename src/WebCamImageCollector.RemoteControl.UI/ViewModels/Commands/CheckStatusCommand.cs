using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class CheckStatusCommand : AsyncCommand
    {
        private readonly IClient client;
        private readonly ClientOverviewViewModel viewModel;

        public CheckStatusCommand(IClient client, ClientOverviewViewModel viewModel)
        {
            Ensure.NotNull(client, "client");
            Ensure.NotNull(viewModel, "viewModel");
            this.client = client;
            this.viewModel = viewModel;
        }

        protected override bool CanExecuteOverride()
        {
            return !viewModel.IsStatusLoading;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            viewModel.IsStatusLoading = true;
            ClientRunningInfo response = await client.IsRunningAsync(cancellationToken);
            viewModel.IsRunning = response.Running;
            viewModel.IsStatusLoading = false;
        }
    }
}
