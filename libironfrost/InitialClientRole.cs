namespace Ironfrost
{
    /// <summary>
    ///   A client role that listens to the initial responses of a client
    ///   and swaps itself out for the correct role after receiving the
    ///   IAMA response.
    /// </summary>
    public class InitialClientRole : IClientRole
    {
        public event RoleChangeHandler Change;

        public event MessageSendHandler SendMessage
        {
            add { }
            remove { }
        }

        /// <summary>
        ///   Event fired when the role gets an OHAI response.
        /// </summary>
        public event OhaiHandler Ohai;

        /// <summary>
        ///   The OHAI response from the server, if any.
        /// </summary>
        private Ohai ohai;

        /// <summary>
        ///   Gets the name of the Bifrost role associated with this
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
                return string.Empty;
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
            IClientRole newRole = RoleFromName(args[0]);
            Change?.Invoke(this, newRole);
        }
        
        /// <summary>
        ///   Selects a <c>IClientRole</c> based on a role string.
        ///   <para>
        ///       If the role is not known, we return <c>ErrorClientRole</c>.
        ///   </para>
        /// </summary>
        /// <param name="roleName">
        ///     The name of the intended role.
        /// </param>
        /// <returns>
        ///     An <c>IClientRole</c> that best matches <paramref name="roleName"/>.
        /// </returns>
        private IClientRole RoleFromName(string roleName)
        {
            switch (roleName)
            {
                case "player/file":
                    return new PlayerClientRole();
                default:
                    return new ErrorClientRole(ClientError.UnknownRole, roleName);
            }
        }
    }
}