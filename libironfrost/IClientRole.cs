namespace ironfrost
{
    /// <summary>
    ///   Delegate for hooking up role change notifications.
    /// </summary>
    /// <param name="newRole">
    ///   The intended new <c>ClientRole</c>.
    /// </param>
    public delegate void RoleChangeHandler(IClientRole newrole);

    /// <summary>
    ///   Interface for client roles.
    ///
    ///   <para>
    ///     <c>IClientRole</c>s listen for messages, act on them (sending
    ///     events, updating state, etc.), and send messages back to
    ///     subscribers.
    ///   </para>
    /// </summary>
    public interface IClientRole
    {
        /// <summary>
        ///   Event fired when the <c>IClientRole</c> wants to send a
        ///   message.
        /// </summary>
        event MessageSendHandler SendMessage;

        /// <summary>
        ///   Event fired when the <c>IClientRole</c> has received a
        ///   message.
        /// </summary>
        event MessageSendHandler RecvMessage;

        /// <summary>
        ///   Event fired when the <c>IClientRole</c> wants to be replaced.
        /// </summary>
        event RoleChangeHandler Change;

        /// <summary>
        ///   Handles the given message from a <c>Client</c>.
        /// </summary>
        /// <param name="msg">
        ///   The message to handle.
        /// </param>
        void HandleMessage(Message msg);
    }
}