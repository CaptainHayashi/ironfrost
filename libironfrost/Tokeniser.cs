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
        private delegate ByteProcessor ByteProcessor(byte b, ref List<List<string>> lines);
        private ByteProcessor processByte;

        /// <summary>
        ///     The line currently being processed.
        ///     
        ///     <para>
        ///         This persists across calls to <c>Feed</c>.
        ///     </para>
        /// </summary>
        List<List<byte>> currentLine = new List<List<byte>>();

        /// <summary>
        ///     The word currently being processed.
        ///     
        ///     <para>
        ///         This persists across calls to <c>Feed</c>.
        ///     </para>
        /// </summary>
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

        /// <summary>
        ///     Pushes the byte <paramref name="b"/> onto the current word.
        /// </summary>
        /// <param name="b">
        ///     The byte to push onto the current word.
        /// </param>
        private void PushByte(byte b)
        {
            inWord = true;
            currentWord.Add(b);
        }

        /// <summary>
        ///     Pushes the current word onto the current line.
        /// 
        ///     <para>
        ///         This clears the current word, ready to accept another word.
        ///     </para>
        /// </summary>
        private void PushWord()
        {
            // Don't add a word unless we're in one.
            if (!inWord) return;
            inWord = false;

            currentLine.Add(new List<byte>(currentWord));
            currentWord.Clear();
        }

        /// <summary>
        ///     Pushes the current line onto the list of outgoing lines.
        /// 
        ///     <para>
        ///         This clears the current line, ready to accept another line.
        ///     </para>
        /// </summary>
        /// <param name="lines">
        ///     The list of lines currently being built.
        /// </param>
        private void PushLine(ref List<List<string>> lines)
        {
            // We might still be in a word, in which case we treat the end of a
            // line as the end of the word too.
            PushWord();

            List<string> line = new List<string>();

            foreach (List<byte> bword in currentLine)
            {
                line.Add(new string(Encoding.UTF8.GetChars(bword.ToArray())));
            }
            lines.Add(line);

            currentLine.Clear();
        }

        /// <summary>
        ///   Processes the byte <paramref name="r" />, in unquoted mode.
        /// </summary>
        /// <param name="r">
        ///   The byte to process.
        /// </param>
        /// <param name="lines">
        ///     The list of lines currently being built.
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
        private ByteProcessor ProcessUnquotedByte(byte r, ref List<List<string>> lines)
        {
            if ('\n' == r)
            {
                PushLine(ref lines);
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
                return (byte s, ref List<List<string>> ls) => { PushByte(s); return processByte; };
            }
            else if (char.IsWhiteSpace((char)r))
            {
                PushWord();
            }

            PushByte(r);
            return ProcessUnquotedByte;
        }

        /// <summary>
        ///   Processes the byte <paramref name="r" />, in single-quoted mode.
        /// </summary>
        /// <param name="r">
        ///   The byte to process.
        /// </param>
        /// <param name="lines">
        ///     The list of lines currently being built.
        /// </param>
        /// <returns>
        ///   The method to use to process the next byte.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     Single-quoted mode means that the byte is processed as follows:
        ///     <list>
        ///       <item>
        ///         <description>
        ///           if the byte is a single quote, we enter unquoted mode;
        ///         </description>
        ///         <description>
        ///           else, we add the byte to the current word.
        ///         </description>
        ///       </item>
        ///     </list>
        ///   </para>
        /// </remarks>
        private ByteProcessor ProcessSQuotedByte(byte r, ref List<List<string>> lines)
        {
            if ('\'' == r)
            {
                return ProcessUnquotedByte;
            }

            PushByte(r);
            return ProcessSQuotedByte;
        }

        /// <summary>
        ///   Processes the byte <paramref name="r" />, in double-quoted mode.
        /// </summary>
        /// <param name="r">
        ///   The byte to process.
        /// </param>
        /// <param name="lines">
        ///     The list of lines currently being built.
        /// </param>
        /// <returns>
        ///   The method to use to process the next byte.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     Double-quoted mode means that the byte is processed as follows:
        ///     <list>
        ///       <item>
        ///         <description>
        ///           if the byte is a double quote, we enter unquoted mode;
        ///         </description>
        ///         <description>
        ///           if the byte is a backslash, we prepare to output the
        ///           next byte verbatim;
        ///         </description>
        ///         <description>
        ///           else, we add the byte to the current word.
        ///         </description>
        ///       </item>
        ///     </list>
        ///   </para>
        /// </remarks>
        private ByteProcessor ProcessDQuotedByte(byte r, ref List<List<string>> lines)
        {
            if ('\"' == r)
            {
                return ProcessUnquotedByte;
            }
            else if ('\\' == r)
            {
                return (byte s, ref List<List<string>> ls) => { PushByte(s); return processByte; };
            }

            PushByte(r);
            return ProcessDQuotedByte;
        }

        /// <summary>
        ///     Feeds the sequence of bytes in <paramref name="raw"/>
        ///     into the tokeniser.
        /// </summary>
        /// <param name="raw">
        ///     The sequence of bytes to tokenise.
        /// </param>
        /// <returns>
        ///     Any lines that completed due to the bytes in
        ///     <paramref name="raw"/>.
        /// </returns>
        public List<List<string>> Feed(IEnumerable<byte> raw)
        {
            var lines = new List<List<string>>();

            foreach (byte r in raw)
            {
                processByte = processByte(r, ref lines);
            }

            return lines;
        }
    }
}
