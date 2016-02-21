using System.Collections.Generic;
using System.Linq;

namespace ironfrost
{
    public class Message
    {
        public string Tag { get; }
        public string Word { get; }
        public string[] Args { get; }

        public Message(IEnumerable<string> words)
        {
            Tag = words.First();
            Word = words.Skip(1).First();
            Args = words.Skip(2).ToArray();
        }
    }
}
