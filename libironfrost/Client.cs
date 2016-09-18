//-----------------------------------------------------------------------
// <copyright file="Client.cs" company="University Radio York">
//     Copyright 2016 University Radio York, licenced under MIT.
// </copyright>
//-----------------------------------------------------------------------
namespace Ironfrost
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Net.Sockets;

    /// <summary>
    ///     A high-level client.
    ///     <para>
    ///         A <see cref="Client"/> combines an <see cref="IClientRole"/>
    ///         and a <see cref="IClientSocket"/> to represent an entire
    ///         client.
    ///      </para>
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        /// <summary>
        ///     The <see cref="IClientSocket"/> used to talk to the server
        ///     itself.
        /// </summary>
        private IClientSocket socket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="socket">
        ///     The <c>IClientSocket</c> to read to and write from.
        /// </param>
        /// <param name="role">
        ///     The <c>Role</c> to pass messages to and from.
        /// </param>
        public Client(IClientSocket socket, IClientRole role)
        {
            this.socket = socket;
            this.socket.LineEvent += ProcessLine;

            ChangeRole(null, role);
        }
        
        /// <summary>
        ///     Event sent when the <see cref="Client"/> receives a message.
        /// </summary>
        public event MessageSendHandler RecvMessage;

        /// <summary>
        ///     Event sent when a property changes on the <c>Client</c>.
        ///     <para>
        ///         This will be <see cref="Role"/> or <see cref="Name"/>.
        ///     </para>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets the current client role.
        /// </summary>
        public IClientRole Role { get; private set; }

        /// <summary>
        ///   Gets the name of this Client.
        /// </summary>
        public string Name
        {
            get
            {
                return socket.Name;
            }
        }

        /// <summary>
        ///     Builds a new <c>Client</c> with the given host and port.
        /// </summary>
        /// <param name="host">
        ///     The hostname on which the target server is listening.
        /// </param>
        /// <param name="port">
        ///     The port on which the target server is listening.
        /// </param>
        /// <returns>
        ///     A <c>Client</c> with sensible values for its socket and
        ///     role, based on <paramref name="host"/> and
        ///     <paramref name="port"/>.
        /// </returns>
        public static Client FromHostAndPort(string host, ushort port)
        {
            var tok = new Tokeniser();

            IClientSocket socket;
            IClientRole role;

            try
            {
                var client = new TcpClient(AddressFamily.InterNetwork);
                var t = client.ConnectAsync(host, port);
                t.Wait();

                socket = new TCPClientSocket($"{host}:{port}", client, tok);
                role = new InitialClientRole();
            }
            catch (SocketException e)
            {
                socket = new NullClientSocket();
                role = new ErrorClientRole(ClientError.CannotConnect, e.Message);
            }
            catch (System.AggregateException e)
            {
                socket = new NullClientSocket();

                var msgs = new List<string>();
                foreach (var x in e.InnerExceptions) msgs.Add(x.Message);
                var msg = string.Join("; ", msgs);

                role = new ErrorClientRole(ClientError.CannotConnect, msg);
            }

            return new Client(socket, role);
        }

        /// <summary>
        ///     Runs the <see cref="Client"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the running <see cref="Client"/>.
        /// </returns>
        public async Task RunAsync()
        {
            bool running = true;
            while (running)
            {
                running = await socket.ReadAsync();
            }

            // TODO(CaptainHayashi): signal EOF
        }

        /// <summary>
        ///   Processes a line from the <c>TCPClientSocket</c>.
        /// </summary>
        /// <param name="line">
        ///   The line to process.
        /// </param>
        private void ProcessLine(List<string> line)
        {
            var msg = new Message(line);

            RecvMessage?.Invoke(this, msg);
        }

        /// <summary>
        ///     Event handler for sending messages to the socket.
        /// </summary>
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
        /// <param name="oldRole">
        ///     The existing <c>ClientRole</c>.
        /// </param>
        /// <param name="newRole">
        ///     The new <c>ClientRole</c>.
        /// </param>
        private void ChangeRole(IClientRole oldRole, IClientRole newRole)
        {
            if (oldRole != null)
            {
                oldRole.SendMessage -= MessageSocket;
                oldRole.Change -= ChangeRole;
                RecvMessage -= oldRole.HandleMessage;
            }

            Role = newRole;

            if (newRole != null)
            {
                Role.SendMessage += MessageSocket;
                Role.Change += ChangeRole;
                RecvMessage += Role.HandleMessage;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Role"));
        }
    }
}