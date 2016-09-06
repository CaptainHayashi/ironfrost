using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Ironfrost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Console : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl SnapIn { get; private set; }

        public string ClientName { get; private set; }

        public void Ohai(Ohai ohai)
        {
            ClientName = $"{ohai.clientID}@{ClientName}/{ohai.serverID} [{ohai.protocolID}]";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClientName"));
        }

        public void Change(IClientRole newRole)
        {
            UserControl ctl = null;
            if (newRole is InitialClientRole)
            {
                ctl = new InitialControl();
                var rl = newRole as InitialClientRole;
                rl.Ohai += (obj, ohai) => Dispatcher.Invoke(() => Ohai(ohai));
            }
            else if (newRole is PlayerClientRole)
            {
                ctl = new PlayerControl(this, newRole as PlayerClientRole);
            }
            else
            {
                ctl = new ErrorControl(newRole);
            }

            SnapIn = ctl;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SnapIn"));
        }

        public Console(string name, IClientRole role)
        {
            InitializeComponent();

            ClientName = name;

            DataContext = this;

            Change(role);
        }
    }
}
