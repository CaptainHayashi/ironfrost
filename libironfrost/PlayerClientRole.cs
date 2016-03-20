using System.ComponentModel;

namespace ironfrost
{
    /// <summary>
    ///   A client role for Bifrost 'player/file' roles.
    /// </summary>
    public class PlayerClientRole : IClientRole, INotifyPropertyChanged
    {
        /// <summary>
        ///   Enumeration of possible player client states.
        /// </summary>
        public enum PlayerState
        {
            /// <summary>
            ///   The player has no file loaded.
            /// </summary>
            Ejected,
            /// <summary>
            ///   The player has a file loaded, but it has stopped.
            /// </summary>
            Stopped,
            /// <summary>
            ///   The player has a file loaded, and it is playing.
            /// </summary>
            Playing,
            /// <summary>
            ///   We don't yet know what the state of the client is.
            /// </summary>
            Unknown
        }

        public event RoleChangeHandler Change;
        public event MessageSendHandler RecvMessage;
        public event MessageSendHandler SendMessage;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   The name of the file currently loaded.
        /// </summary>
        public string Fload { get; private set; }

        /// <summary>
        ///   The current position.
        /// </summary>
        public uint Pos { get; private set; }

        /// <summary>
        ///   The current state of the player.
        /// </summary>
        public PlayerState State { get; private set; }

        /// <summary>
        ///   The name of the Bifrost role associated with this
        ///   <c>PlayerRole</c>.
        ///   
        ///   <para>
        ///     This is always 'player/file'.
        ///   </para>
        /// </summary>
        public string Name
        {
            get
            {
                return "player/file";
            }
        }

        public void HandleMessage(Message msg)
        {
            if (RecvMessage != null)
            {
                RecvMessage(this, msg);
            }

            switch (msg.Word)
            {
                case "FLOAD":
                    Fload = msg.Args[0];
                    Notify("Fload");

                    /* Assume that a FLOAD that wasn't preceded by an EJECT
                     * doesn't mean we've actually loaded anything.
                     *
                     * Conversely, if it was, then we can assume we've moved to
                     * Stopped with a position of 0.  The server might tell us
                     * this anyway, but it doesn't need to.
                     */
                    if (State == PlayerState.Ejected)
                    {
                        State = PlayerState.Stopped;
                        Notify("State");

                        if (Pos != 0)
                        {
                            Pos = 0;
                            Notify("Pos");
                        }
                    }
                    break;
                case "PLAY":
                    UpdateState(PlayerState.Playing);
                    break;
                case "STOP":
                    UpdateState(PlayerState.Stopped);
                    break;
                case "END":
                    // TODO(CaptainHayashi): End signal.
                    UpdateState(PlayerState.Stopped);
                    UpdatePos(0);
                    break;
                case "POS":
                    // TODO(CaptainHayashi): gracefully handle broken POS strings.
                    UpdatePos(uint.Parse(msg.Args[0]));
                    break;
                default:
                    break;
            }
        }

        private void UpdateState(PlayerState newState)
        {
            if (State != newState)
            {
                State = newState;
                Notify("State");
            }
        }

        private void UpdatePos(uint newPos)
        {
            if (Pos != newPos)
            {
                Pos = newPos;
                Notify("Pos");
            }
        }

        private void Notify(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}