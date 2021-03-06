﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Ironfrost
{
    /// <summary>
    ///   A low-level TCP Bifrost client.
    ///
    ///   <para>
    ///     <c>Client</c> allows Bifrost messages to be read and written as
    ///     lists of strings over TCP.
    ///   </para>
    /// </summary>
    public class TCPClientSocket : IClientSocket
    {
        /// <summary>
        ///   The fixed size of the input buffer, in bytes.
        /// <summary> 
        const int InputBufferSize = 4096;
        
        /// <summary>
        ///   The initial size of the output buffer, in bytes.
        /// <summary> 
        const int OutputBufferSize = 1024;
        
        public delegate Task RespondAsync(IEnumerable<string> command);

        public string Name { get; }

        /// <summary>
        ///   Event fired when a <c>ReadAsync</c> gets a line.
        /// </summary>
        public event Tokeniser.LineHandler LineEvent;

        private TcpClient client;
        private NetworkStream stream;
        private byte[] buffer;
        private Tokeniser tok;

        public TCPClientSocket(string name, TcpClient client, Tokeniser tok)
        {
            Name = name;
            this.client = client;
            stream = client.GetStream();
            buffer = new byte[InputBufferSize];

            this.tok = tok;
            this.tok.LineEvent += GotLine;
        }

        private void GotLine(List<string> line)
        {
            LineEvent?.Invoke(line);
        }

        public async Task<bool> ReadAsync()
        {
            int nread = await stream.ReadAsync(buffer, 0, buffer.Length);
            tok.Feed(buffer.Take(nread));
            return (nread > 0);
        }

        public async Task WriteAsync(IEnumerable<string> command)
        {
            using (var mbuf = new MemoryStream(OutputBufferSize))
            {
                var packer = new Packer(mbuf);
                packer.Pack(command);
                mbuf.Seek(0, SeekOrigin.Begin);
                await mbuf.CopyToAsync(stream);
            }
        }
    }
}
