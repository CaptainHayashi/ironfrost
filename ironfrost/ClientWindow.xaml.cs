using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace ironfrost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ClientSocket.RespondAsync respond;

        public string ClientName { get; private set; }
        public ObservableCollection<Message> Msgs { get; }

        public void NewMessage(Message msg)
        {
            Msgs.Add(msg);
        }
        
        public void Ohai(Ohai ohai)
        {
            ClientName = $"{ohai.clientID}@{ClientName}/{ohai.serverID} [{ohai.protocolID}]";
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs("ClientName"));
            }
        }

        public ClientWindow(string name, ClientSocket.RespondAsync rs)
        {
            InitializeComponent();

            ClientName = name;
            Msgs = new ObservableCollection<Message>();
            DataContext = this;

            respond = rs;
        }

        /// <summary>
        ///    Creates a new, fresh tag.
        /// </summary>
        /// <returns>
        ///     A tag string.  This string should be globally unique.
        /// </returns>
        private string FreshTag()
        {
            return Guid.NewGuid().ToString();
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Audio files (.mp3, .flac, .wav, .ogg)|*.mp3;*.wav;*.flac;*.ogg";
            bool? ok = dlg.ShowDialog(this);

            if (ok == true)
            {
                respond(new Message(FreshTag(), "fload", dlg.FileName));
            }
        }

        private void btnEject_Click(object sender, RoutedEventArgs e)
        {
            respond(new Message(FreshTag(), "eject"));
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            respond(new Message(FreshTag(), "play"));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            respond(new Message(FreshTag(), "stop"));
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            respond(new Message(FreshTag(), "end"));
        }
    }
}
