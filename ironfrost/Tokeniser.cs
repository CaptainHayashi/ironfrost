using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ironfrost
{
    public class Tokeniser
    {
        private bool inWord = false;

        private delegate void ByteProcessor(byte b);
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

        private void ProcessUnquotedByte(byte r)
        {
            if ('\n' == r)
            {
                PushLine();
            }
            else if ('\'' == r)
            {
                inWord = true;
                processByte = ProcessSQuotedByte;
            }
            else if ('\"' == r)
            {
                inWord = true;
                processByte = ProcessDQuotedByte;
            }
            else if ('\\' == r)
            {
                EscapeNextByte();
            }
            else if (char.IsWhiteSpace((char) r))
            {
                PushWord();
            }
            else
            {
                PushByte(r);
            }
        }

        private void ProcessSQuotedByte(byte r)
        {
            if ('\'' == r)
            {
                processByte = ProcessUnquotedByte;
            }
            else
            {
                PushByte(r);
            }
        }

        private void ProcessDQuotedByte(byte r)
        {
            if ('\"' == r)
            {
                processByte = ProcessUnquotedByte;
            }
            else if ('\\' == r)
            {
                EscapeNextByte();
            }
            else
            {
                PushByte(r);
            }
        }

        private void EscapeNextByte()
        {
            var eb = processByte;
            processByte = (r) => { PushByte(r); processByte = eb; };
        }

        public List<List<string>> Feed(IEnumerable<byte> raw)
        {
            foreach (byte r in raw)
            {
                processByte(r);
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
