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
    public class DeleteLocalCommand : NavigateCommand
    {
        public DeleteLocalCommand()
            :  base(typeof(Overview))
        { }

        public override bool CanExecute()
        {
            return true;
        }

        public override void Execute()
        {
            ClientRepository repository = new ClientRepository();
            repository.DeleteLocal();

            base.Execute();
        }
    }
}
