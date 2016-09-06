//-----------------------------------------------------------------------
// <copyright file="ErrorClientRole.cs" company="University Radio York">
//     Copyright 2016 University Radio York, licenced under MIT.
// </copyright>
//-----------------------------------------------------------------------
namespace Ironfrost
{
    /// <summary>
    ///     Enumeration of errors an <see cref="ErrorClientRole"/> can
    ///     represent.
    /// </summary>
    public enum ClientError
    {
        /// <summary>
        ///     The external server reported a role we don't support.
        ///     <para>
        ///         Details contains one string: the reported role.
        ///     </para>
        /// </summary>
        UnknownRole,

        /// <summary>
        ///     Trying to connect to the Client reported an exception.
        ///     <para>
        ///         Details contains one string: the exception string.
        ///     </para>
        /// </summary>
        CannotConnect
    }

    /// <summary>
    ///     A client role that represents an error.
    /// </summary>
    public class ErrorClientRole : IClientRole
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorClientRole"/> class.
        /// </summary>
        /// <param name="type">
        ///     The type of the error this role represents.
        /// </param>
        /// <param name="details">
        ///     Details to report with the error.
        /// </param>
        public ErrorClientRole(ClientError type, params string[] details)
        {
            Type = type;
            Details = details;
        }

        /// <summary>
        ///     This event is ignored for <see cref="ErrorClientRole"/>s.
        /// </summary>
        public event RoleChangeHandler Change
        {
            add { }
            remove { }
        }

        /// <summary>
        ///     This event is ignored for <see cref="ErrorClientRole"/>s.
        /// </summary>
        public event MessageSendHandler SendMessage
        {
            add { }
            remove { }
        }

        /// <summary>
        ///     Gets the name of this <see cref="ErrorClientRole"/>.
        /// </summary>
        public string Name
        {
            get { return "(error)"; }
        }

        /// <summary>
        ///     Gets the error message associated with the error this
        ///     <see cref="ErrorClientRole"/> is storing.
        /// </summary>
        public string Message
        {
            get
            {
                switch (Type)
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

        /// <summary>
        ///     Gets the type enumeration associated with the error this
        ///     <see cref="ErrorClientRole"/> is storing.
        /// </summary>
        public ClientError Type { get; }

        /// <summary>
        ///     Gets the detail strings associated with the error this
        ///     <see cref="ErrorClientRole"/> is storing.
        /// </summary>
        public string[] Details { get; }

        /// <summary>
        ///     Handles (ignores) a message from the client.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="msg">The message (to be ignored).</param>
        public void HandleMessage(object sender, Message msg)
        {
            // Intentionally ignore responses.
            // TODO(CaptainHayashi): raise an error?
        }
    }
}