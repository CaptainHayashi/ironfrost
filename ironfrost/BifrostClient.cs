using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ironfrost
{
    public class BifrostClient
    {
        public delegate Task RespondAsync(List<string> command);

        private System.Net.Sockets.TcpClient client;
        private System.Net.Sockets.NetworkStream stream;
        private byte[] buffer;
        private Tokeniser tok;

        public BifrostClient(string host, ushort port, Tokeniser tok)
        {
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
            int count = 0;
            byte[] packed = tok.Pack(command, out count);
            await stream.WriteAsync(packed, 0, count);
        }
    }
}
