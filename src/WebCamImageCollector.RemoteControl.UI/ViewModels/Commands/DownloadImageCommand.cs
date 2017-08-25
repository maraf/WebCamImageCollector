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
    public class DownloadImageCommand : AsyncCommand
    {
        private readonly IClient client;
        private readonly Func<ImageQuality> qualityGetter;

        public event Action<ClientImageModel> Completed;

        public DownloadImageCommand(IClient client, Func<ImageQuality> qualityGetter)
        {
            Ensure.NotNull(client, "client");
            Ensure.NotNull(qualityGetter, "qualityGetter");
            this.client = client;
            this.qualityGetter = qualityGetter;
        }

        protected override bool CanExecuteOverride()
        {
            return true;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            ClientImageModel image = await client.DownloadLatestAsync(qualityGetter(), cancellationToken);
            Completed?.Invoke(image);
        }
    }
}
