using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ironfrost
{
    public class Tokeniser
    {
        private bool inWord = false;

        private delegate ByteProcessor ByteProcessor(byte b);
        private ByteProcessor processByte;

        List<List<List<byte>>> currentLines = new List<List<List<byte>>>();
        List<List<byte>> currentLine = new List<List<byte>>();
        List<byte> currentWord = new List<byte>();

        public Tokeniser()
        {
            processByte = ProcessUnquotedByte;
        }

        private void PushByte(byte b)
        {
            inWord = true;
            currentWord.Add(b);
        }

        private void PushWord()
        {
            // Don't add a word unless we're in one.
            if (!inWord) return;
            inWord = false;

            currentLine.Add(new List<byte>(currentWord));
            currentWord.Clear();
        }

        private void PushLine()
        {
            // We might still be in a word, in which case we treat the end of a
            // line as the end of the word too.
            PushWord();

            currentLines.Add(new List<List<byte>>(currentLine));
            currentLine.Clear();
        }

        /// <summary>
        ///   Processes the byte <paramref name="r" />, in unquoted mode.
        /// </summary>
        /// <param name="r">
        ///   The byte to process.
        /// </param>
        /// <returns>
        ///   The method to use to process the next byte.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     Unquoted mode means that the byte is processed as follows:
        ///     <list>
        ///       <item>
        ///         <description>
        ///           if the byte is a newline, we end the current line
        ///           and append it to the list of lines to output;
        ///         </description>
        ///         <description>
        ///           if the byte is a single or double quote, we enter
        ///           the respective quote mode;
        ///         </description>
        ///         <description>
        ///           if the byte is a backslash, we prepare to output the
        ///           next byte verbatim;
        ///         </description>
        ///         <description>
        ///           if the byte is whitespace, we end the current word
        ///           and append it to the current line;
        ///         </description>
        ///         <description>
        ///           else, we add the byte to the current word.
        ///         </description>
        ///       </item>
        ///     </list>
        ///   </para>
        /// </remarks>
        private ByteProcessor ProcessUnquotedByte(byte r)
        {
            if ('\n' == r)
            {
                PushLine();
            }
            else if ('\'' == r)
            {
                inWord = true;
                return ProcessSQuotedByte;
            }
            else if ('\"' == r)
            {
                inWord = true;
                return ProcessDQuotedByte;
            }
            else if ('\\' == r)
            {
                return (s) => { PushByte(s); return processByte; };
            }
            else if (char.IsWhiteSpace((char) r))
            {
                PushWord();
            }

            PushByte(r);
            return ProcessUnquotedByte;
        }

        private ByteProcessor ProcessSQuotedByte(byte r)
        {
            if ('\'' == r)
            {
                return ProcessUnquotedByte;
            }

            PushByte(r);
            return ProcessSQuotedByte;
        }

        private ByteProcessor ProcessDQuotedByte(byte r)
        {
            if ('\"' == r)
            {
                return ProcessUnquotedByte;
            }
            else if ('\\' == r)
            {
                return (s) => { PushByte(s); return processByte; };
            }

            PushByte(r);
            return ProcessDQuotedByte;
        }

        public List<List<string>> Feed(IEnumerable<byte> raw)
        {
            foreach (byte r in raw)
            {
                processByte = processByte(r);
            }

            List<List<string>> lines = new List<List<string>>();
            foreach (List<List<byte>> bline in currentLines)
            {
                List<string> line = new List<string>();

                foreach (List<byte> bword in bline)
                {
                    line.Add(new string(Encoding.UTF8.GetChars(bword.ToArray())));
                }

                lines.Add(line);
            }

            currentLines.Clear();
            return lines;
        }

        public byte[] Pack(List<string> line, out int count)
        {
            var buf = new System.IO.MemoryStream(1024);
            foreach (var word in line)
            {
                PackWord(ref buf, word);
                // We'll fix the off-by one here later.
                buf.WriteByte((byte)' ');
            }

            byte[] bs = buf.GetBuffer();
            count = checked ((int) buf.Length);
            bs[count - 1] = (byte)'\n';

            return bs;
        }

        void PackWord(ref System.IO.MemoryStream buf, string word)
        {
            // We see why we have a second buf later.
            var ibuf = new System.IO.MemoryStream(word.Count() * 2);

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
                } else
                {
                    ibuf.WriteByte(b);
                }
            }

            // Now, copy ibuf to buf.
            // If we've marked ourselves as single-quoting, surround ibuf in single
            // quotes.
            if (escaping)
            {
                buf.WriteByte((byte)'\'');
            }
            ibuf.WriteTo(buf);
            if (escaping)
            {
                buf.WriteByte((byte)'\'');
            }
        }
    }
}
