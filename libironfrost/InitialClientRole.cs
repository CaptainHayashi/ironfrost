using System;

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

        public void HandleMessage(Message msg)
        {
            // TODO(CaptainHayashi): actually do something here.
            if (RecvMessage != null)
            {
                RecvMessage(this, msg);
            }
        }
    }
}