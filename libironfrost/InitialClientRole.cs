using System;

namespace ironfrost
{
    /// <summary>
    ///   A client role that listens to the initial responses of a client
    ///   and swaps itself out for the correct role after receiving the
    ///   IAMA response.
    /// </summary>
    public class InitialClientRole : IClientRole
    {
        public event RoleChangeHandler Change;
        public event MessageSendHandler SendMessage;

        /// <summary>
        ///   Event fired when the role gets an OHAI response.
        /// </summary>
        public event OhaiHandler Ohai;

        /// <summary>
        ///   The OHAI response from the server, if any.
        /// </summary>
        private Ohai ohai;

        /// <summary>
        ///   The name of the Bifrost role associated with this
        ///   <c>InitialClientRole</c>.
        ///   
        ///   <para>
        ///     We don't actually know the role yet, so we give the empty
        ///     string.
        ///   </para>
        /// </summary>
        public string Name
        {
            get
            {
                return "";
            }
        }

        public void HandleMessage(object sender, Message msg)
        {
            if (msg.Word == "OHAI")
            {
                HandleOhai(msg.Args);
            }
            else if (msg.Word == "IAMA")
            {
                HandleIama(msg.Args);
            }
        }

        /// <summary>
        ///   Handles an OHAI message.
        /// </summary>
        /// <param name="args">
        ///   The message arguments, which should be of size 3.
        /// </param>
        private void HandleOhai(string[] args)
        {
            // TODO(CaptainHayashi): check size
            // TODO(CaptainHayashi): refuse double OHAI
            ohai.clientID = args[0];
            ohai.protocolID = args[1];
            ohai.serverID = args[2];

            if (Ohai != null)
            {
                Ohai(this, ohai);
            }
        }

        /// <summary>
        ///   Handles an IAMA message.
        /// </summary>
        /// <param name="args">
        ///   The message arguments, which should be of size 1.
        /// </param>
        private void HandleIama(string[] args)
        {
            // TODO(CaptainHayashi): check size

            IClientRole newRole = new NullClientRole(args[0]);

            switch (args[0])
            {
                case "player/file":
                    newRole = new PlayerClientRole();
                    break;
                default:
                    break;
            }

            if (Change != null)
            {
                Change(this, newRole);
            }
        }
    }
}