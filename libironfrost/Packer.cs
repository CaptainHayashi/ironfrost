using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ironfrost
{
    /// <summary>
    ///   Class for packing Bifrost messages into byte streams.
    /// </summary>
    class Packer
    {
        private Stream ostream;

        public Packer(Stream os)
        {
            ostream = os;
        }

        public void Pack(IEnumerable<string> line)
        {
            PackWord(line.First());

            foreach (var word in line.Skip(1))
            {
                ostream.WriteByte((byte)' ');
                PackWord(word);
            }
            ostream.WriteByte((byte)'\n');
        }

        void PackWord(string word)
        {
            // We see why we have a second buf later.
            var ibuf = new MemoryStream(word.Count() * 2);

            byte[] wordbytes = Encoding.UTF8.GetBytes(word);

            bool escaping = false;

            foreach (var b in wordbytes)
            {
                var c = (char)b;

                // These are the characters (including all whitespace, via
                // isspace())  whose presence means we need to single-quote
                // escape the argument.
                bool is_escaper = c == '"' || c == '\'' || c == '\\';
                if (char.IsWhiteSpace(c) || is_escaper) escaping = true;

                // Since we use single-quote escaping, the only thing we need
                // to escape by itself is single quotes, which are replaced by
                // the sequence '\'' (break out of single quotes, escape a
                // single quote, then re-enter single quotes).
                if (c == '\'')
                {
                    ibuf.WriteByte((byte)'\'');
                    ibuf.WriteByte((byte)'\\');
                    ibuf.WriteByte((byte)'\'');
                    ibuf.WriteByte((byte)'\'');
                }
                else
                {
                    ibuf.WriteByte(b);
                }
            }

            // Now, copy ibuf to buf.
            // If we've marked ourselves as single-quoting, surround ibuf in single
            // quotes.
            if (escaping)
            {
                ostream.WriteByte((byte)'\'');
            }
            ibuf.WriteTo(ostream);
            if (escaping)
            {
                ostream.WriteByte((byte)'\'');
            }
        }
    }
}