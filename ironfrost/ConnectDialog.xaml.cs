using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ironfrost
{
    /// <summary>
    ///   Container for connection information.
    /// </summary>
    public struct ConnectionDetails
    {
        /// <summary>
        ///   The host to connect to.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///   The port to connect to.
        /// </summary>
        public int Port { get; set; }
    }

    /// <summary>
    /// Interaction logic for ConnectDialog.xaml
    /// </summary>
    public partial class ConnectDialog : Window
    {
        public ConnectionDetails Connection;

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
            Connection.Host = defaultHost;
            Connection.Port = defaultPort;
            DataContext = Connection;

            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
