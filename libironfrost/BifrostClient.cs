using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ironfrost
{
    public class BifrostClient
    {
        public delegate Task RespondAsync(List<string> command);

        public string Name { get; }

        private System.Net.Sockets.TcpClient client;
        private System.Net.Sockets.NetworkStream stream;
        private byte[] buffer;
        private Tokeniser tok;

        public BifrostClient(string host, ushort port, Tokeniser tok)
        {
            Name = $"{host}:{port}";

            client = new System.Net.Sockets.TcpClient(host, port);
            stream = client.GetStream();
            buffer = new byte[4096];
            this.tok = tok;
        }

        public async Task<List<List<string>>> ReadAsync()
        {
            int nread = await stream.ReadAsync(buffer, 0, buffer.Length);
            return tok.Feed(buffer.Take(nread));
        }

        public async Task WriteAsync(List<string> command)
        {
            using (var mbuf = new MemoryStream(1024)) {
                var packer = new Packer(mbuf);
                packer.Pack(command);
                mbuf.Seek(0, SeekOrigin.Begin);
                await mbuf.CopyToAsync(stream);
            }
        }
    }
}
