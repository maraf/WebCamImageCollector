using Neptuo.Observables;
using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebCamImageCollector.RemoteControl.ViewModels
{
    public class OverviewViewModel : ObservableObject
    {
        private ClientOverviewViewModel local;
        public ClientOverviewViewModel Local
        {
            get { return local; }
            private set
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
        public ICommand CreateLocal { get; private set; }

        public OverviewViewModel()
        {
            Remotes = new ObservableCollection<ClientOverviewViewModel>();
        }
    }
}
