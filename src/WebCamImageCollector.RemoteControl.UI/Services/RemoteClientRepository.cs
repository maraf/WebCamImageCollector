using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class RemoteClientRepository
    {
        private readonly List<RemoteClient> storage = new List<RemoteClient>();

        public IEnumerable<RemoteClient> Enumerate()
        {
            return storage;
        }

        public RemoteClientRepository Add(RemoteClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            storage.Add(client);
            return this;
        }

        public RemoteClientRepository Remove(RemoteClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");

            storage.Remove(client);
            return this;
        }

        public void Save()
        {
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            foreach (string name in container.Containers.Keys)
                container.DeleteContainer(name);

            int index = 1;
            foreach (RemoteClient client in storage)
            {
                ApplicationDataContainer clientContainer =  container
                    .CreateContainer(String.Format("Client-{0}", index), ApplicationDataCreateDisposition.Always);

                clientContainer.Values["Url"] = client.Url;
                clientContainer.Values["AuthenticationToken"] = client.AuthenticationToken;
                index++;
            }
        }

        public void Load()
        {
            storage.Clear();

            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            foreach (ApplicationDataContainer clientContainer in container.Containers.Values)
                storage.Add(new RemoteClient((string)clientContainer.Values["Url"], (string)clientContainer.Values["AuthenticationToken"]));
        }
    }
}
