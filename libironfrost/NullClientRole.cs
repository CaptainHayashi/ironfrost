namespace ironfrost
{
    /// <summary>
    ///   A client role that ignores all requests and responses.
    /// </summary>
    public class NullClientRole : IClientRole
    {
        public event RoleChangeHandler Change;
        public event MessageSendHandler RecvMessage;
        public event MessageSendHandler SendMessage;

        public string Name { get; }

        /// <summary>
        ///   Constructs a new <c>NullClientRole</c>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Bifrost role being mocked up by the null role.
        /// </param>
        public NullClientRole(string name)
        {
            Name = name;
        }

        public void HandleMessage(Message msg)
        {
            RecvMessage?.Invoke(this, msg);

            // Intentionally ignore responses.
            // TODO(CaptainHayashi): raise an error?
        }
    }
}