using System.ComponentModel;

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


        /// <summary>
        ///   Holder for the client console, if it is open.
        /// </summary>
        private Console console = null;

        /// <summary>
        ///   Lock used to serialise accesses to <c>console</c>.
        /// </summary>
        private object consoleLock = new object();


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
        }

        private void OnClientChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Role")
            {
                // Lock to prevent races with other attempts to use the window.
                lock (consoleLock)
                {
                    var newRole = Client.Role;
                    var wnd = console;
                    wnd?.Dispatcher.Invoke(() => wnd.Change(newRole));
                }
            }
        }

        /// <summary>
        ///   If a window is open for this client, retrieve it.
        ///   Else, open a new one.
        /// </summary>
        /// <returns>
        ///   A <c>ClientWindow</c> on this tracker's <c>Client</c>.
        /// </returns>
        public Console GetWindow()
        {
            if (console == null)
            {
                // Lock to prevent races with things trying to use the window.
                lock (consoleLock)
                {
                    console = new Console(Client.Name, Client.Role);
                    console.Closed += (obj, e) => { console = null; };
                    console.Show();
                }
            }

            return console;
        }
    }
}
