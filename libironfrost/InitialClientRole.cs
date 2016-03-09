namespace ironfrost
{
    /// <summary>
    ///     A client role that listens to the initial responses of a client
    ///     and swaps itself out for the correct role after receiving the
    ///     IAMA response.
    /// </summary>
    public class InitialClientRole : IClientRole
    {
        public event RoleChangeHandler Change;
        public event MessageSendHandler RecvMessage;
        public event MessageSendHandler SendMessage;

        /// <summary>
        ///    The OHAI response from the server, if any.
        /// </summary>
        private Ohai ohai;

        public void HandleMessage(Message msg)
        {
            if (RecvMessage != null)
            {
                RecvMessage(this, msg);
            }
            
            if (msg.Word == "OHAI") {
                HandleOhai(msg.Args);
            }
        }
        
        /// <summary>
        ///     Handles an OHAI message.
        /// </summary>
        /// <param name="args" />
        ///     The message arguments which should be of size 3.
        /// </param>
        private void HandleOhai(string[] args) {
            // TODO(CaptainHayashi): check size
            // TODO(CaptainHayashi): refuse double OHAI
            this.ohai.clientID = args[0];
            this.ohai.protocolID = args[1];
            this.ohai.serverID = args[2];    
        }
    }
}