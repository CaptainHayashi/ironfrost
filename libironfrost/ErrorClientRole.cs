namespace ironfrost
{
    /// <summary>
    ///     Enumeration of errors an <c>ErrorClientRole</c> can represent.
    /// </summary>
    public enum ClientError
    {
        /// <summary>
        ///   The Bifrost server reported a role we don't support.
        ///   
        ///   <para>
        ///     Details contains one string: the reported role.
        ///   </para>
        /// </summary>
        UnknownRole,

        /// <summary>
        ///   Trying to connect to the Client reported an exception.
        ///   
        ///   <para>
        ///     Details contains one string: the exception string.
        ///   </para>
        /// </summary>
        CannotConnect
    }

    /// <summary>
    ///   A client role that represents an error..
    /// </summary>
    public class ErrorClientRole : IClientRole
    {
        public event RoleChangeHandler Change;
        public event MessageSendHandler SendMessage;

        public string Name { get { return "(error)"; } }

        public string Message
        {
            get
            {
                switch(Type)
                {
                    case ClientError.CannotConnect:
                        return $"Could not connect to the Bifrost server: {Details[0]}";
                    case ClientError.UnknownRole:
                        return $"Bifrost server returned unknown role: {Details[0]}";
                    default:
                        return $"Unknown error: {Type.ToString()}";
                }
            }
        }

        public ClientError Type { get; }
        public string[] Details { get; }

        /// <summary>
        ///   Constructs a new <c>ErrorClientRole</c>.
        /// </summary>
        /// <param name="name">
        ///   The name of the Bifrost role, if known.
        /// </param>
        /// <param name="details">
        ///   Details to report with the error.
        /// </param>
        public ErrorClientRole(ClientError type, params string[] details)
        {
            Type = type;
            Details = details;
        }

        public void HandleMessage(object sender, Message msg)
        {
            // Intentionally ignore responses.
            // TODO(CaptainHayashi): raise an error?
        }
    }
}