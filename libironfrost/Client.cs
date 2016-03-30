using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;

namespace ironfrost
{
    /// <summary>
    ///     A high-level Bifrost client.
    ///
    ///     <para>
    ///         A <c>Client</c> combines a <c>ClientRole</c> and a
    ///         <c>ClientSocket</c> to represent an entire Bifrost client,
    ///         using the <c>ClientRole</c> for behaviour and
    ///         <c>ClientSocket</c> for communication respectively.
    ///      </para>
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        private ClientSocket socket;

        public event PropertyChangedEventHandler PropertyChanged;

        public IClientRole Role { get; private set; }

        /// <summary>
        ///   The name of this Client.
        /// </summary>
        public string Name
        {
            get
            {
                return socket.Name;
            }
        }

        /// <summary>
        ///     Creates a new <c>Client</c>.
        /// </summary>
        /// <param name="socket">
        ///     The <c>ClientSocket</c> to read to and write from.
        /// </param>
        /// <param name="role">
        ///     The <c>Role</c> to pass messages to and from.
        /// </param>
        public Client(ClientSocket socket, IClientRole role)
        {
            this.socket = socket;
            this.socket.LineEvent += ProcessLine;

            ChangeRole(null, role);
        }

        /// <summary>
        ///   Processes a line from the <c>ClientSocket</c>.
        /// </summary>
        /// <param name="line">
        ///   The line to process.
        /// </param>
        private void ProcessLine(List<string> line)
        {
            var msg = new Message(line);
            Role.HandleMessage(msg);
        }

        /// <summary>
        ///     Runs the <c>Client</c>.
        /// </summary>
        public async Task RunAsync()
        {
            while (true)
            {
                await socket.ReadAsync();
            }
        }

        /// <summary>
        ///     Event handler for sending messages to the socket.
        /// <summary>
        /// <param name="obj">
        ///     The originating object (usually <c>role</c>).
        /// </param>
        /// <param name="msg">
        ///     The message to be sent to the socket.
        /// </param>
        private void MessageSocket(object obj, Message msg)
        {
            Task.Run(() => socket.WriteAsync(msg));
        }

        /// <summary>
        ///     Event handler for changing roles.
        /// </summary>
        /// <param name="newRole">
        ///     The new <c>ClientRole</c>.
        /// </param>
        private void ChangeRole(IClientRole oldRole, IClientRole newRole)
        {
            if (oldRole != null)
            {
                oldRole.SendMessage -= MessageSocket;
                oldRole.Change -= ChangeRole;
            }

            Role = newRole;

            if (newRole != null)
            {
                Role.SendMessage += MessageSocket;
                Role.Change += ChangeRole;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Role"));
        }
    }
}