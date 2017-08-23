using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.Views;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class DeleteRemoteCommand : NavigateCommand
    {
        private readonly Guid key;

        public DeleteRemoteCommand(Guid key)
            : base(typeof(Overview))
        {
            this.key = key;
        }

        public override void Execute()
        {
            ClientRepository repository = new ClientRepository();
            repository.DeleteRemote(key);

            base.Execute();
        }
    }
}
