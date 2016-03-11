using System.Collections.Generic;
using System.Text;

namespace ironfrost
{
    /// <summary>
    ///   A Bifrost protocol tokeniser.
    /// </summary>
    public class Tokeniser
    {
        /// <summary>
        ///   Delegate used to hook up line events.
        /// </summary>
        public delegate void LineHandler(List<string> line);

        /// <summary>
        ///   Event fired when the <c>Tokeniser</c> finishes a line.
        /// </summary>
        public event LineHandler LineEvent;

        /// <summary>
        ///   Delegate for functions that process one byte, and return
        ///   the processor for the next byte.
        /// </summary>
        private delegate ByteProcessor ByteProcessor(byte b);

        /// <summary>
        ///   The <c>ByteProcessor</c> that will process the next byte.
        /// </summary>
        private ByteProcessor processByte;

        /// <summary>
        ///   The line currently being processed.
        ///     
        ///   <para>
        ///     This persists across calls to <c>Feed</c>.
        ///   </para>
        /// </summary>
        List<List<byte>> currentLine = new List<List<byte>>();

        /// <summary>
        ///   The word currently being processed.
        ///     
        ///   <para>
        ///     This persists across calls to <c>Feed</c>.
        ///   </para>
        /// </summary>
        List<byte> currentWord = new List<byte>();

        /// <summary>
        ///   Constructs a new <c>Tokeniser</c>.
        ///   <para>
        ///     The new <c>Tokeniser</c> starts in whitespace mode, with no
        ///     lines ready to output.
        ///   </para>
        /// </summary>
        public Tokeniser()
        {
            processByte = ProcessWhitespaceByte;
        }

        /// <summary>
        ///   Pushes the byte <paramref name="b"/> onto the current word.
        /// </summary>
        /// <param name="b">
        ///   The byte to push onto the current word.
        /// </param>
        private void PushByte(byte b)
        {
            currentWord.Add(b);
        }

        /// <summary>
        ///   Pushes the current word onto the current line.
        /// 
        ///   <para>
        ///     This clears the current word, ready to accept another word.
        ///   </para>
        /// </summary>
        private void PushWord()
        {
            currentLine.Add(new List<byte>(currentWord));
            currentWord.Clear();
        }

        /// <summary>
        ///   Pushes the current line to <c>LineEvent</c> listeners.
        /// 
        ///   <para>
        ///     This clears the current line, ready to accept another line.
        ///   </para>
        /// </summary>
        private void PushLine()
        {
            // We might still be in a word, in which case we treat the end of a
            // line as the end of the word too.
            PushWord();

            // Only bother packaging the line up if someone's listening.
            if (LineEvent != null)
            {
                List<string> line = new List<string>();

                foreach (List<byte> bword in currentLine)
                {
                    line.Add(new string(Encoding.UTF8.GetChars(bword.ToArray())));
                }

                if (LineEvent != null) LineEvent(line);
            }

            currentLine.Clear();
        }

        /// <summary>
        ///   Processes the byte <paramref name="r" />, in whitespace mode.
        /// </summary>
        /// <param name="r">
        ///   The byte to process.
        /// </param>
        /// <returns>
        ///   The method to use to process the next byte.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///     Whitespace mode means that the byte is processed as follows:
        ///     <list>
        ///       <item>
        ///         <description>
        ///           if the byte is whitespace, we ignore it and stay in
        ///           whitespace mode;
        ///         </description>
        ///         <description>
        ///           else, we behave as if we were in unquoted mode.
        ///         </description>
        ///       </item>
        ///     </list>
        ///   </para>
        /// </remarks>
        private ByteProcessor ProcessWhitespaceByte(byte r)
        {
            if (char.IsWhiteSpace((char)r))
            {
                return ProcessWhitespaceByte;
            }

            // The (r) is deliberate--we're switching to unquoted mode *now*.
            return ProcessUnquotedByte(r);
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
        ///           and append it to the current line, switching to whitespace
        ///           mode;
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
                return ProcessSQuotedByte;
            }
            else if ('\"' == r)
            {
                return ProcessDQuotedByte;
            }
            else if ('\\' == r)
            {
                return (byte s) => { PushByte(s); return processByte; };
            }
            else if (char.IsWhiteSpace((char)r))
            {
                PushWord();
                return ProcessWhitespaceByte;
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
        private ByteProcessor ProcessSQuotedByte(byte r)
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
        private ByteProcessor ProcessDQuotedByte(byte r)
        {
            if ('\"' == r)
            {
                return ProcessUnquotedByte;
            }
            else if ('\\' == r)
            {
                return (byte s) => { PushByte(s); return processByte; };
            }

            PushByte(r);
            return ProcessDQuotedByte;
        }

        /// <summary>
        ///   Feeds the sequence of bytes in <paramref name="raw"/>
        ///   into the tokeniser.
        /// </summary>
        /// <param name="raw">
        ///   The sequence of bytes to tokenise.
        /// </param>
        public void Feed(IEnumerable<byte> raw)
        {
            var lines = new List<List<string>>();

            foreach (byte r in raw)
            {
                processByte = processByte(r);
            }
        }
    }
}
