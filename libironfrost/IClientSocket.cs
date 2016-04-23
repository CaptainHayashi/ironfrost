using System.Collections.Generic;
using System.Threading.Tasks;

namespace ironfrost
{
    /// <summary>
    ///   Interface for low-level socket connections to Bifrost servers.
    /// </summary>
    public interface IClientSocket
    {
        string Name { get; }

        event Tokeniser.LineHandler LineEvent;

        /// <summary>
        ///   Requests a read on this <c>IClientSocket</c>.
        ///
        ///   <para>
        ///     This may trigger <c>LineEvent</c> if the socket has managed to
        ///     read a complete line.
        ///   </para>
        /// </summary>
        /// <returns>
        ///   True if the socket still has more to read.
        /// </returns>
        Task<bool> ReadAsync();
        Task WriteAsync(IEnumerable<string> command);
    }
}