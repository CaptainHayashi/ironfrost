using System.Windows;

namespace ironfrost
{
    /// <summary>
    ///   The inspector window.
    /// </summary>
    public partial class Inspector : Window
    {
        private ClientTracker client;

        public Inspector(ClientTracker client)
        {
            this.client = client;
            DataContext = this.client;

            InitializeComponent();
        }
    }
}
