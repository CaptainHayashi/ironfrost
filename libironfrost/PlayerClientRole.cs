using System;
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
        ///   The current position, as a <c>TimeSpan</c>.
        ///
        ///    <para>
        ///        This conversion loses precision.
        ///    </para>
        /// </summary>
        public TimeSpan PosSpan
        {
            get
            {
                return TimeSpan.FromMilliseconds(Pos / 1000);
            }
        }

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
                    UpdateFload(msg.Args[0]);

                    /* Assume that a FLOAD that wasn't preceded by an EJECT
                       doesn't mean we've actually loaded anything.

                       Conversely, if it was, then we can assume we've moved to
                       Stopped with a position of 0.  The server might tell us
                       this anyway, but it doesn't need to. */
                    if (State == PlayerState.Ejected)
                    {
                        State = PlayerState.Stopped;
                        Notify("State");

                        UpdatePos(0);
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
                case "EJECT":
                    UpdateState(PlayerState.Ejected);
                    UpdateFload(null);
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

        /// <summary>
        ///   Asks this role to send a 'fload' message.
        /// </summary>
        /// <param name="file">
        ///   The file to load.
        /// </param>
        public void RequestFload(string file)
        {
            Send("fload", file);
        }

        /// <summary>
        ///   Asks this role to send a 'pos' message.
        /// </summary>
        /// <param name="pos">
        ///   The position to request, in microseconds.
        /// </param>
        public void RequestPos(uint pos)
        {
            Send("pos", pos.ToString());
        }

        /// <summary>
        ///   Asks this role to send a 'play' message.
        /// </summary>
        public void RequestPlay()
        {
            Send("play");
        }

        /// <summary>
        ///   Asks this role to send a 'stop' message.
        /// </summary>
        public void RequestStop()
        {
            Send("stop");
        }

        /// <summary>
        ///   Asks this role to send an 'eject' message.
        /// </summary>
        public void RequestEject()
        {
            Send("eject");
        }

        /// <summary>
        ///   Asks this role to send an 'end' message.
        /// </summary>
        public void RequestEnd()
        {
            Send("end");
        }

        private void Send(string word, params string[] args)
        {
            // TODO(CaptainHayashi): return task for checking if this succeeded
            SendMessage?.Invoke(this, new Message(Message.FreshTag(), word, args));
        }

        private void UpdateFload(string newFload)
        {
            if (Fload != newFload)
            {
                Fload = newFload;
                Notify("Fload");
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
                Notify("PosSpan");
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