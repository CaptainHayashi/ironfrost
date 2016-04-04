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

        Task ReadAsync();
        Task WriteAsync(IEnumerable<string> command);
    }
}