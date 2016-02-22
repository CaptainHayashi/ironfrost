using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ironfrost
{
    /// <summary>
    ///   A Bifrost protocol tokeniser.
    /// </summary>
    public class Tokeniser
    {
        private bool inWord = false;

        /// <summary>
        ///   Delegate for functions that process one byte, and return
        ///   the processor for the next byte.
        /// </summary>
        private delegate ByteProcessor ByteProcessor(byte b);
        private ByteProcessor processByte;

        List<List<List<byte>>> currentLines = new List<List<List<byte>>>();
        List<List<byte>> currentLine = new List<List<byte>>();
        List<byte> currentWord = new List<byte>();

        /// <summary>
        ///   Constructs a new <c>Tokeniser</c>.
        ///   <para>
        ///     The new <c>Tokeniser</c> starts in unquoted mode, with no
        ///     lines ready to output.
        ///   </para>
        /// </summary>
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
    }
}
