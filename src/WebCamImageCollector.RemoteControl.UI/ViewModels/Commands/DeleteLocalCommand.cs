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
    public class DeleteLocalCommand : DeleteCommandBase
    {
        public DeleteLocalCommand()
            :  base(typeof(Overview), "Do you really want to delete local client?")
        { }

        public override bool CanExecute()
        {
            return true;
        }

        protected override bool ExecuteOverride()
        {
            ClientRepository repository = new ClientRepository();
            repository.DeleteLocal();

            return true;
        }
    }
}
