using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ironfrost
{
    /// <summary>
    ///   A low-level Bifrost client.
    ///
    ///   <para>
    ///     <c>Client</c> allows Bifrost messages to be read and written as
    ///     lists of strings over TCP.
    ///   </para>
    /// </summary>
    public class ClientSocket
    {
        public delegate Task RespondAsync(IEnumerable<string> command);

        public string Name { get; }

        /// <summary>
        ///   Event fired when a <c>ReadAsync</c> gets a line.
        /// </summary>
        public event Tokeniser.LineHandler LineEvent;

        private System.Net.Sockets.TcpClient client;
        private System.Net.Sockets.NetworkStream stream;
        private byte[] buffer;
        private Tokeniser tok;

        public ClientSocket(string host, ushort port, Tokeniser tok)
        {
            Name = $"{host}:{port}";

            client = new System.Net.Sockets.TcpClient(host, port);
            stream = client.GetStream();
            buffer = new byte[4096];
            this.tok = tok;
            this.tok.LineEvent += GotLine;
        }

        private void GotLine(List<string> line)
        {
            LineEvent?.Invoke(line);
        }

        public async Task ReadAsync()
        {
            int nread = await stream.ReadAsync(buffer, 0, buffer.Length);
            tok.Feed(buffer.Take(nread));
        }

        public async Task WriteAsync(IEnumerable<string> command)
        {
            using (var mbuf = new MemoryStream(1024))
            {
                var packer = new Packer(mbuf);
                packer.Pack(command);
                mbuf.Seek(0, SeekOrigin.Begin);
                await mbuf.CopyToAsync(stream);
            }
        }
    }
}
