using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ironfrost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private Client.RespondAsync respond;

        public string ClientName { get; }
        public ObservableCollection<Message> Msgs { get; }

        public void NewMessage(Message msg)
        {
            Msgs.Add(msg);
        }

        public ClientWindow(string name, Client.RespondAsync rs)
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
            dlg.Filter = "Audio files (.mp3, .flac, .wav)|*.mp3;*.wav;*.flac";
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
