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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ironfrost
{
    /// <summary>
    ///     Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        /// <summary>
        ///   The client role to which this <c>PlayerControl</c> is attached.
        /// </summary>
        public IClientRole Role { get; }

        public PlayerControl(IClientRole role)
        {
            Role = role;
            DataContext = role;

            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //var dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.Filter = "Audio files (.mp3, .flac, .wav, .ogg)|*.mp3;*.wav;*.flac;*.ogg";
            //bool? ok = dlg.ShowDialog(this);

            //if (ok == true)
            //{
            //    respond(new Message(FreshTag(), "fload", dlg.FileName));
            //}
        }

        private void btnEject_Click(object sender, RoutedEventArgs e)
        {
            //respond(new Message(FreshTag(), "eject"));
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            //respond(new Message(FreshTag(), "play"));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //respond(new Message(FreshTag(), "stop"));
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            //respond(new Message(FreshTag(), "end"));
        }
    }
}
