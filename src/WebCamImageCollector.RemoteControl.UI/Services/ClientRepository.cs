using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WebCamImageCollector.RemoteControl.Services
{
    public class ClientRepository
    {
        private readonly Dictionary<Guid, RemoteClient> remoteClients = new Dictionary<Guid, RemoteClient>();
        private LocalClient localClient;

        public ClientRepository()
        {
            Load();
        }

        public IEnumerable<RemoteClient> EnumerateRemote()
        {
            return remoteClients.Values;
        }

        public IClient Find(Guid key)
        {
            RemoteClient remote = FindRemote(key);
            if(remote == null)
            {
                LocalClient local = FindLocal();
                if (local != null && local.Key == key)
                    return local;
            }

            return remote;
        }

        public LocalClient FindLocal()
        {
            return localClient;
        }

        public RemoteClient FindRemote(Guid key)
        {
            RemoteClient client;
            if (remoteClients.TryGetValue(key, out client))
                return client;

            return null;
        }

        public RemoteClient CreateRemote(string name, string url, string authenticationToken)
        {
            Guid key = Guid.NewGuid();
            RemoteClient client = new RemoteClient(key, name, url, authenticationToken);
            remoteClients.Add(key, client);
            Save();
            return client;
        }

        public ClientRepository DeleteRemote(Guid key)
        {
            if (remoteClients.Remove(key))
                Save();

            return this;
        }

        public LocalClient CreateOrReplaceLocal(int port, string authenticationToken, int interval, int delay)
        {
            if (localClient == null || localClient.Port != port || localClient.AuthenticationToken != authenticationToken || localClient.Interval != interval || localClient.Delay != delay)
            {
                Guid key = Guid.NewGuid();
                localClient = new LocalClient(key, port, authenticationToken, interval, delay);
                Save();
            }

            return localClient;
        }

        public void DeleteLocal()
        {
            if (localClient != null)
            {
                localClient = null;
                Save();
            }
        }

        private void Save()
        {
            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            root.DeleteContainer("Remote");

            ApplicationDataContainer remote = root
                .CreateContainer("Remote", ApplicationDataCreateDisposition.Always);

            foreach (RemoteClient client in remoteClients.Values)
            {
                ApplicationDataContainer item = remote
                    .CreateContainer(client.Key.ToString(), ApplicationDataCreateDisposition.Always);

                item.Values["Name"] = client.Name;
                item.Values["Url"] = client.Url;
                item.Values["AuthenticationToken"] = client.AuthenticationToken;
            }

            root.DeleteContainer("Local");
            if (localClient != null)
            {
                ApplicationDataContainer localContainer = root.CreateContainer("Local", ApplicationDataCreateDisposition.Always);
                localContainer.Values["Key"] = localClient.Key;
                localContainer.Values["Port"] = localClient.Port;
                localContainer.Values["AuthenticationToken"] = localClient.AuthenticationToken;
                localContainer.Values["Interval"] = localClient.Interval;
                localContainer.Values["Delay"] = localClient.Delay;
            }
        }

        private void Load()
        {
            remoteClients.Clear();

            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;

            ApplicationDataContainer remote;
            if (root.Containers.TryGetValue("Remote", out remote))
            {
                foreach (ApplicationDataContainer item in remote.Containers.Values)
                {
                    Guid key = Guid.Parse(item.Name);
                    remoteClients.Add(key, new RemoteClient(
                        key,
                        (string)item.Values["Name"],
                        (string)item.Values["Url"],
                        (string)item.Values["AuthenticationToken"]
                    ));
                }
            }

            ApplicationDataContainer local;
            if (root.Containers.TryGetValue("Local", out local))
            {
                //if (!local.Values.ContainsKey("key"))
                //{
                //    root.DeleteContainer("Local");
                //    return;
                //}

                Guid key = (Guid)local.Values["Key"];
                localClient = new LocalClient(
                    key,
                    (int)local.Values["Port"],
                    (string)local.Values["AuthenticationToken"],
                    (int)local.Values["Interval"],
                    (int)local.Values["Delay"]
                );
            }
        }
    }
}
