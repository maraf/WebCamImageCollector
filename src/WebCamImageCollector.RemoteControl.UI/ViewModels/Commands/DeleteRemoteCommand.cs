using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCamImageCollector.RemoteControl.Services;
using WebCamImageCollector.RemoteControl.Views;
using Windows.UI.Popups;

namespace WebCamImageCollector.RemoteControl.ViewModels.Commands
{
    public class DeleteRemoteCommand : DeleteCommandBase
    {
        private readonly Guid key;

        public DeleteRemoteCommand(Guid key)
            : base(typeof(Overview), "Do you really want to delete remote client?")
        {
            this.key = key;
        }

        protected override bool ExecuteOverride()
        {
            ClientRepository repository = new ClientRepository();
            repository.DeleteRemote(key);

            return true;
        }
    }
}
