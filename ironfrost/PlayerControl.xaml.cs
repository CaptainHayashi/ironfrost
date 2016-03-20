using System.Windows.Controls;
using System.Windows.Input;

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

            Role.PropertyChanged += RolePropertyChanged;

            InitializeComponent();
        }

        private void RolePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            /* Most of the Role's properties are tracked by the view;
               the only one we need to do something on is if the state changes,
               to tell the control to check whether its buttons need disabling
               and reenabling. */
            if (e.PropertyName == "State")
            {
                Dispatcher.Invoke(() =>
                {
                    CommandManager.InvalidateRequerySuggested();
                });
            }
        }

        private void ExecuteLoad(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Audio files (.mp3, .flac, .wav, .ogg)|*.mp3;*.wav;*.flac;*.ogg";
            bool? ok = dlg.ShowDialog();

            if (ok == true)
            {
                Role.RequestFload(dlg.FileName);
            }
        }

        private void CanEject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State != PlayerClientRole.PlayerState.Ejected;
        }

        private void ExecuteEject(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestEject();
        }

        private void CanPlay(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State == PlayerClientRole.PlayerState.Stopped;
        }

        private void ExecutePlay(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestPlay();
        }

        private void CanStop(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State == PlayerClientRole.PlayerState.Playing;
        }

        private void ExecuteStop(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestStop();
        }

        private void CanEnd(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State != PlayerClientRole.PlayerState.Ejected;
        }

        private void ExecuteEnd(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestEnd();
        }
    }
}
