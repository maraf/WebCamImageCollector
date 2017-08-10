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
    public class StopCommand : AsyncCommand
    {
        private readonly IClient client;

        public StopCommand(IClient client)
        {
            Ensure.NotNull(client, "client");
            this.client = client;
        }

        protected override bool CanExecuteOverride()
        {
            return true;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return client.StopAsync(cancellationToken);
        }
    }
}
