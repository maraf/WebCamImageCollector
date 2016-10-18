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

        public LocalClient LocalClient { get; private set; }

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

        public RemoteClientRepository SetLocal(LocalClient client)
        {
            LocalClient = client;
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
                ApplicationDataContainer clientContainer = container
                    .CreateContainer(String.Format("Client-{0}", index), ApplicationDataCreateDisposition.Always);

                clientContainer.Values["Url"] = client.Url;
                clientContainer.Values["AuthenticationToken"] = client.AuthenticationToken;
                index++;
            }

            if (LocalClient != null)
            {
                ApplicationDataContainer localContainer = container.CreateContainer("Local", ApplicationDataCreateDisposition.Always);
                localContainer.Values["Port"] = LocalClient.Port;
                localContainer.Values["AuthenticationToken"] = LocalClient.AuthenticationToken;
            }
        }

        public void Load()
        {
            storage.Clear();

            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            foreach (ApplicationDataContainer clientContainer in container.Containers.Values)
            {
                if (clientContainer.Name.StartsWith("Client-"))
                {
                    storage.Add(new RemoteClient(
                        (string)clientContainer.Values["Url"],
                        (string)clientContainer.Values["AuthenticationToken"]
                    ));
                }
                else if (clientContainer.Name == "Local")
                {
                    LocalClient = new LocalClient(
                        (int)clientContainer.Values["Port"],
                        (string)clientContainer.Values["AuthenticationToken"]
                    );
                }
            }
        }
    }
}
