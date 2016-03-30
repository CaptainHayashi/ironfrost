using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ironfrost
{
    /// <summary>
    ///   Tracker containing a Client and its associated window.
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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        ///   The set of clients being tracked by Ironfrost.
        /// </summary>
        public ObservableCollection<ClientTracker> Clients { get; }

        public MainWindow()
        {
            Clients = new ObservableCollection<ClientTracker>();
            DataContext = this;  // TODO(CaptainHayashi): split this out

            InitializeComponent();
        }

        private void ExecuteConnect(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var connectDialog = new ConnectDialog("localhost", 1350);
            if (connectDialog.ShowDialog() == true)
            {
                string host = connectDialog.Connection.Host;
                ushort port = (ushort)connectDialog.Connection.Port;
                Connect(host, port);
            }
        }

        private void ExecuteOpenClientWindow(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var client = e.Parameter as ClientTracker;
            client.GetWindow().Focus();
        }

        private void Connect(string host, ushort port)
        {
            var tok = new Tokeniser();

            ClientSocket bc;
            try
            {
                bc = new ClientSocket(host, port, tok);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                System.Console.Out.WriteLine(ex.ToString());
                throw;
            }

            var rl = new InitialClientRole();

            var client = new Client(bc, rl);
            var ct = new ClientTracker(client);
            Clients.Add(ct);

            /* Don't start the client until we're watching it.
               Otherwise, we could race on it changing role. */ 
            Task.Run(client.RunAsync);

            ct.GetWindow().Focus();
        }
    }
}
