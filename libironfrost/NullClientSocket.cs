using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ironfrost
{
    /// <summary>
    ///   Dummy Bifrost client that never reports messages and bins any
    ///   sent to it.
    /// </summary>
    public class NullClientSocket : IClientSocket
    {
        /// <summary>
        ///   Dummy event, never used.
        /// </summary>
        public event Tokeniser.LineHandler LineEvent
        {
            add { }
            remove { }
        }

        public string Name { get { return "(null)"; } }

        public NullClientSocket()
        {
            // Intentionally left blank.
        }

        public Task<bool> ReadAsync()
        {
            // Intentionally left blank.
            return Task.FromResult(true);
        }

        public Task WriteAsync(IEnumerable<string> command)
        {
            // Intentionally left blank.
            return Task.FromResult(0);
        }
    }
}
