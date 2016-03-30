using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ironfrost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Console : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string ClientName { get; private set; }
        public ObservableCollection<Message> Msgs { get; }

        public void NewMessage(Message msg)
        {
            Msgs.Add(msg);
        }

        public void Ohai(Ohai ohai)
        {
            ClientName = $"{ohai.clientID}@{ClientName}/{ohai.serverID} [{ohai.protocolID}]";
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ClientName"));
            }
        }

        public void Change(IClientRole newRole)
        {
            // TODO(CaptainHayashi): transition to ClientTracker.
            newRole.RecvMessage += (obj, msg) => Dispatcher.Invoke(() => NewMessage(msg));

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
                ctl = new NullControl(newRole);
            }

            tabControls.Content = ctl;
        }

        public Console(string name, IClientRole role)
        {
            InitializeComponent();

            ClientName = name;
            Msgs = new ObservableCollection<Message>();

            DataContext = this;

            Change(role);
        }
    }
}
