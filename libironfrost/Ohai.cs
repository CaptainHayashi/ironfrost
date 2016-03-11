namespace ironfrost
{
    /// <summary>
    ///   Delegate for hooking up OHAI notifications.
    /// </summary>
    /// <param name="obj">
    ///   The originating object.
    /// </param>
    /// <param name="ohai">
    ///   The information from the OHAI response.
    /// </param>
    public delegate void OhaiHandler(object obj, Ohai ohai);
    
    /// <summary>
    ///   Structure for <c>OHAI</c> response information.
    /// </summary>
    public struct Ohai
    {
        /// <summary>
        ///   The client ID assigned by the server.
        /// </summary>
        public string clientID;
        
        /// <summary>
        ///   The name, including version, of the server.
        /// </summary>
        public string serverID;
        
        /// <summary>
        ///   The name, including version, of the protocol.
        /// </summary>
        public string protocolID;
        
        /// <summary>
        ///   Constructs an <c>Ohai</c>.
        /// </summary>
        /// <param name="clientID">
        ///   The client ID to use in the <c>Ohai</c>.
        /// </param>
        /// <param name="serverID">
        ///   The server ID to use in the <c>Ohai</c>.
        /// </param>
        /// <param name="protocolID">
        ///   The protocol ID to use in the <c>Ohai</c>.
        /// </param>
        public Ohai(string clientID, string serverID, string protocolID) {
            this.clientID = clientID;
            this.serverID = serverID;
            this.protocolID = protocolID;
        }
    }
}