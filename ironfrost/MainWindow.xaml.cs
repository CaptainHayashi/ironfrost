using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Ironfrost
{
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

        private void ExecuteOpenConsole(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var client = e.Parameter as ClientTracker;
            client.GetConsole().Focus();
        }

        private void ExecuteOpenInspector(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var client = e.Parameter as ClientTracker;
            client.GetInspector().Focus();
        }

        private void Connect(string host, ushort port)
        {
            var client = Client.FromHostAndPort(host, port);

            var ct = new ClientTracker(client);
            Clients.Add(ct);

            /* Don't start the client until we're watching it.
               Otherwise, we could race on it changing role. */ 
            Task.Run(client.RunAsync);

            ct.GetConsole().Focus();
        }
    }
}
