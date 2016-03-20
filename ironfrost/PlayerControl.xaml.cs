using System;
using System.Collections.Generic;
using System.Globalization;
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
        public PlayerClientRole Role { get; }

        /// <summary>
        ///   The window in which this <c>PlayerControl</c> is embedded.
        /// </summary>
        private ClientWindow wnd;

        /// <summary>
        ///   Constructs a <c>PlayerControl</c>.
        /// </summary>
        /// <param name="wnd">
        ///   The window in which this <c>PlayerControl</c> is embedded.
        /// </param>
        /// <param name="role">
        ///   The role to be used for listening to responses and sending
        ///   changes.
        /// </param>
        public PlayerControl(ClientWindow wnd, PlayerClientRole role)
        {
            this.wnd = wnd;

            Role = role;
            DataContext = role;

            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Audio files (.mp3, .flac, .wav, .ogg)|*.mp3;*.wav;*.flac;*.ogg";
            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                Role.RequestFload(dlg.FileName);
            }
        }

        private void btnEject_Click(object sender, RoutedEventArgs e)
        {
            Role.RequestEject();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Role.RequestPlay();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Role.RequestStop();
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            Role.RequestEnd();
        }
    }
}
