using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Neptuo;
using WebCamImageCollector.RemoteControl.Services;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public partial class DownloadImageCommand : AsyncCommand
    {
        private readonly IClient client;
        private readonly IViewModel viewModel;

        public event Action<ClientImageModel> Completed;
        public event Action Failed;

        public DownloadImageCommand(IClient client, IViewModel viewModel)
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

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                viewModel.IsDownloading = true;

                ClientImageModel image = await client.DownloadLatestAsync(viewModel.Quality, cancellationToken);
                Completed?.Invoke(image);
            }
            catch (ClientException)
            {
                Failed?.Invoke();
            }
            finally
            {
                viewModel.IsDownloading = false;
            }
        }
    }
}
