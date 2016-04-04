using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ironfrost
{
    /// <summary>
    ///   Tracker containing a Client and its associated windows.
    /// </summary>
    public class ClientTracker
    {
        public Client Client { get; }

        public string Name { get { return Client.Name; } }
        public IClientRole Role { get { return Client.Role; } }

        public ObservableCollection<Message> Msgs { get; } = new ObservableCollection<Message>();

        public void NewMessage(object sender, Message msg)
        {
            // This has to be done on the UI dispatcher to avoid races.
            Application.Current.Dispatcher.Invoke(() => Msgs.Add(msg));
        }

        /// <summary>
        ///   Holder for the client console, if it is open.
        /// </summary>
        private Console console = null;

        /// <summary>
        ///   Holder for the client inspector, if it is open.
        /// </summary>
        private Inspector inspector = null;

        /// <summary>
        ///   Constructs a new <c>ClientTracker</c>.
        /// </summary>
        /// <param name="client">
        ///   The <c>ClientTracker</c> to construct.
        /// </param>
        public ClientTracker(Client client)
        {
            Client = client;

            Client.PropertyChanged += OnClientChange;
            Client.RecvMessage += NewMessage;
        }

        private void OnClientChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Role")
            {
                Application.Current.Dispatcher.Invoke(() =>
                    console?.Change(Client.Role)
                );
            }
        }

        /// <summary>
        ///   If a console is open for this client, retrieve it.
        ///   Else, open a new one.
        /// </summary>
        /// <returns>
        ///   A <c>Console</c> for this tracker's <c>Client</c>.
        /// </returns>
        public Console GetConsole()
        {
            if (console == null)
            {
                // Synchronise with the UI thread.
                // This is to prevent races with things trying to use the window.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    console = new Console(Client.Name, Client.Role);
                    console.Closed += (obj, e) => { console = null; };
                    console.Show();
                });
            }

            return console;
        }

        /// <summary>
        ///   If an inspector is open for this client, retrieve it.
        ///   Else, open a new one.
        /// </summary>
        /// <returns>
        ///   An <c>Inspector</c> for this tracker's <c>Client</c>.
        /// </returns>
        public Inspector GetInspector()
        {
            if (inspector == null)
            {
                // Lock to prevent races with things trying to use the window.
                Application.Current.Dispatcher.Invoke(() =>
                {
                    inspector = new Inspector(this);
                    inspector.Closed += (obj, e) => { inspector = null; };
                    inspector.Show();
                });
            }

            return inspector;
        }
    }
}
