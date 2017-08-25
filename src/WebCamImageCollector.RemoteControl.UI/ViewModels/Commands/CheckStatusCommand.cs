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
        public interface IViewModel
        {
            bool? IsRunning { get; set; }
            bool IsStatusLoading { get; set; }
        }

        private readonly IClient client;
        private readonly IViewModel viewModel;

        public CheckStatusCommand(IClient client, IViewModel viewModel)
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
            try
            {
                viewModel.IsStatusLoading = true;
                ClientRunningInfo response = await client.IsRunningAsync(cancellationToken);
                viewModel.IsRunning = response.Running;
            }
            catch (ClientNotAvailableException)
            {
                viewModel.IsRunning = null;
            }
            catch (ClientException)
            {
                viewModel.IsRunning = null;
                throw;
            }
            finally
            {
                viewModel.IsStatusLoading = false;
            }
        }
    }
}
