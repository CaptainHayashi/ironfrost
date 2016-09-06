using System.Windows;

namespace Ironfrost
{
    /// <summary>
    ///   Container for connection information.
    /// </summary>
    public class ConnectionDetails
    {
        /// <summary>
        ///   The host to connect to.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///   The port to connect to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///   Constructs a new <c>ConnectionDetails</c>.
        /// </summary>
        /// <param name="host">
        ///   The hostname to be connected to.
        /// </param>
        /// <param name="port">
        ///   The TCP port on the hostname to connect to.
        /// </param>
        public ConnectionDetails(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }

    /// <summary>
    /// Interaction logic for ConnectDialog.xaml
    /// </summary>
    public partial class ConnectDialog : Window
    {
        public ConnectionDetails Connection { get; }

        /// <summary>
        ///   Constructs a new <c>ConnectDialog</c>.
        /// </summary>
        /// <param name="defaultHost">
        ///   The initial host to display.
        /// </param>
        /// <param name="defaultPort">
        ///   The initial port to display.
        /// </param>
        public ConnectDialog(string defaultHost, int defaultPort)
        {
            Connection = new ConnectionDetails(defaultHost, defaultPort);
            InitializeComponent();
            DataContext = Connection;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
