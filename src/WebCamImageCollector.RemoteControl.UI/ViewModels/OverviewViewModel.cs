using Neptuo.Observables;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WebCamImageCollector.RemoteControl.ViewModels.Commands;
using WebCamImageCollector.RemoteControl.Views;

namespace WebCamImageCollector.RemoteControl.ViewModels
{
    public class OverviewViewModel : ObservableObject
    {
        private ClientOverviewViewModel local;
        public ClientOverviewViewModel Local
        {
            get { return local; }
            set
            {
                if (local != value)
                {
                    local = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<ClientOverviewViewModel> Remotes { get; private set; }

        public ICommand CreateRemote { get; private set; }
        public ICommand EditLocal { get; private set; }

        public OverviewViewModel()
        {
            Remotes = new ObservableCollection<ClientOverviewViewModel>();
            CreateRemote = new NavigateCommand(typeof(RemoteClientEdit));
            EditLocal = new NavigateCommand(typeof(LocalClientEdit));
        }
    }
}
