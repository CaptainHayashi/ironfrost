using System.Collections.Generic;
using System.Threading.Tasks;

namespace ironfrost
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
        public event Tokeniser.LineHandler LineEvent;

        public string Name { get { return "(null)"; } }

        public NullClientSocket()
        {
            // Intentionally left blank.
        }

        public async Task ReadAsync()
        {
            // Intentionally left blank.
        }

        public async Task WriteAsync(IEnumerable<string> command)
        {
            // Intentionally left blank.
        }
    }
}
