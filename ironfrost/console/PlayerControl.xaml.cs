using System.Windows.Controls;
using System.Windows.Input;

namespace Ironfrost
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
        private Console wnd;

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
        public PlayerControl(Console wnd, PlayerClientRole role)
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

        /// <summary>
        ///     Executes the Load command.
        /// </summary>
        /// <param name="sender">The object sending the execute request.</param>
        /// <param name="e">The set of arguments for the execution.</param>
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

        /// <summary>
        ///     Updates the can-execute status of the Eject command.
        /// </summary>
        /// <param name="sender">The object sending the can-execute request.</param>
        /// <param name="e">The target of the update.</param>
        private void CanEject(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State != PlayerClientRole.PlayerState.Ejected;
        }

        /// <summary>
        ///     Executes the Eject command.
        /// </summary>
        /// <param name="sender">The object sending the execute request.</param>
        /// <param name="e">The set of arguments for the execution.</param>
        private void ExecuteEject(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestEject();
        }

        /// <summary>
        ///     Updates the can-execute status of the Play command.
        /// </summary>
        /// <param name="sender">The object sending the can-execute request.</param>
        /// <param name="e">The target of the update.</param>
        private void CanPlay(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State == PlayerClientRole.PlayerState.Stopped;
        }

        /// <summary>
        ///     Executes the Play command.
        /// </summary>
        /// <param name="sender">The object sending the execute request.</param>
        /// <param name="e">The set of arguments for the execution.</param>
        private void ExecutePlay(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestPlay();
        }

        /// <summary>
        ///     Updates the can-execute status of the Stop command.
        /// </summary>
        /// <param name="sender">The object sending the can-execute request.</param>
        /// <param name="e">The target of the update.</param>
        private void CanStop(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State == PlayerClientRole.PlayerState.Playing;
        }

        /// <summary>
        ///     Executes the Stop command.
        /// </summary>
        /// <param name="sender">The object sending the execute request.</param>
        /// <param name="e">The set of arguments for the execution.</param>
        private void ExecuteStop(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestStop();
        }

        /// <summary>
        ///     Updates the can-execute status of the End command.
        /// </summary>
        /// <param name="sender">The object sending the can-execute request.</param>
        /// <param name="e">The target of the update.</param>
        private void CanEnd(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Role.State != PlayerClientRole.PlayerState.Ejected;
        }

        /// <summary>
        ///     Executes the End command.
        /// </summary>
        /// <param name="sender">The object sending the execute request.</param>
        /// <param name="e">The set of arguments for the execution.</param>
        private void ExecuteEnd(object sender, ExecutedRoutedEventArgs e)
        {
            Role.RequestEnd();
        }
    }
}
