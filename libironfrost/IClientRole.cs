//-----------------------------------------------------------------------
// <copyright file="IClientRole.cs" company="University Radio York">
//     Copyright 2016 University Radio York, licenced under MIT.
// </copyright>
//-----------------------------------------------------------------------
namespace Ironfrost
{
    /// <summary>
    ///     Delegate for hooking up role change notifications.
    /// </summary>
    /// <param name="oldRole">
    ///     The current <see cref="IClientRole"/>.
    /// </param>
    /// <param name="newRole">
    ///     The intended new <see cref="IClientRole"/>.
    /// </param>
    public delegate void RoleChangeHandler(IClientRole oldRole, IClientRole newRole);

    /// <summary>
    ///     Interface for client roles.
    ///     <para>
    ///         <see cref="IClientRole"/>s listen for messages, act on them
    ///         (sending events, updating state, etc.), and send messages back
    ///         to subscribers.
    ///     </para>
    /// </summary>
    public interface IClientRole
    {
        /// <summary>
        ///   Event fired when the <c>IClientRole</c> wants to send a
        ///   message.
        /// </summary>
        event MessageSendHandler SendMessage;

        /// <summary>
        ///   Event fired when the <c>IClientRole</c> wants to be replaced.
        /// </summary>
        event RoleChangeHandler Change;

        /// <summary>
        ///   Gets the name of this role.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///   Handles the given message from a <c>Client</c>.
        /// </summary>
        /// <param name="sender">
        ///   The sender of the message.
        /// </param>
        /// <param name="msg">
        ///   The message to handle.
        /// </param>
        void HandleMessage(object sender, Message msg);
    }
}