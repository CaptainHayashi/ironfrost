using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ironfrost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BifrostClient.RespondAsync respond;

        public ObservableCollection<Message> Msgs { get; }

        public void NewMessage(Message msg)
        {
            Msgs.Add(msg);
        }

        public MainWindow(BifrostClient.RespondAsync rs)
        {
            InitializeComponent();
            Msgs = new ObservableCollection<Message>();
            DataContext = this;

            respond = rs;
        }

        private void Respond(string word, IEnumerable<string> args)
        {
            var cmd = new List<string>();

            var tag = Guid.NewGuid();
            cmd.Add(tag.ToString());

            cmd.Add(word);
            foreach (var arg in args)
            {
                cmd.Add(arg);
            }
            respond(cmd);
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Audio files (.mp3, .flac, .wav)|*.mp3;*.wav;*.flac";
            bool? ok = dlg.ShowDialog(this);

            if (ok == true)
            {
                Respond("fload", new[] { dlg.FileName });
            }
        }

        private void btnEject_Click(object sender, RoutedEventArgs e)
        {
            Respond("eject", new string[] { });
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Respond("play", new string[] { });
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Respond("stop", new string[] { });
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            Respond("end", new string[] { });
        }
    }
}
