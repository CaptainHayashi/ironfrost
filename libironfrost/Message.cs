using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ironfrost
{
    /// <summary>
    ///     Delegate for hooking up message send notifications.
    /// </summary>
    /// <param name="obj">
    ///     The originating object.
    /// </param>
    /// <param name="msg">
    ///     The message the originating object wishes to send.
    /// </param>
    public delegate void MessageSendHandler(object obj, Message msg);


    /// <summary>
    ///    A Bifrost message.
    ///    
    ///    <para>
    ///        <c>Message</c> can be constructed from, and used as,
    ///        a string enumerator.
    ///    </para>
    /// </summary>
    public class Message : IEnumerable<string>
    {
        /// <summary>
        ///     The Message's unique identifier.
        ///     
        ///     <para>
        ///         Tags can also be used for routing.
        ///     </para>
        /// </summary>
        public string Tag { get; }

        /// <summary>
        ///     The message word, which identifies the type of message.
        /// </summary>
        public string Word { get; }

        /// <summary>
        ///    The message arguments.
        /// </summary>
        public string[] Args { get; }

        /// <summary>
        ///     Constructs a message from a tag, word, and argument list.
        /// </summary>
        /// <param name="tag">
        ///     The new message's tag.
        /// </param>
        /// <param name="word">
        ///     The new message's message word.
        /// </param>
        /// <param name="args">
        ///     The new message's argument list.
        /// </param>
        public Message(string tag, string word, IEnumerable<string> args)
        {
            Tag = tag;
            Word = word;
            Args = args.ToArray();
        }

        /// <summary>
        ///     Constructs a message from a tag, word, and variadic arguments.
        /// </summary>
        /// <param name="tag">
        ///     The new message's tag.
        /// </param>
        /// <param name="word">
        ///     The new message's message word.
        /// </param>
        /// <param name="args">
        ///     Every argument after <paramref name="word"/> is treated as a
        ///     message argument.
        /// </param>
        public Message(string tag, string word, params string[] args)
            : this(tag, word, (IEnumerable<string>)args)
        { }

        /// <summary>
        ///     Constructs a message from a word list.
        ///     
        ///     <para>
        ///         The first word is used as the message's tag.
        ///         The second word is used as the message word.
        ///         All further words are used as arguments.
        ///     </para>
        /// </summary>
        /// <param name="words">
        ///     The list of words.
        ///     There must be at least 2 words.
        /// </param>
        public Message(IEnumerable<string> words)
            : this(words.First(), words.ElementAt(1), words.Skip(2))
        { }

        /// <summary>
        ///     Gets a string enumerator for this <c>Message</c>.
        /// </summary>
        /// <returns>
        ///     A string <c>IEnumerator</c>.
        ///     The enumerator yields the <c>Message</c>'s tag, word,
        ///     and arguments.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            yield return Tag;
            yield return Word;
            foreach (var arg in Args)
            {
                yield return arg;
            }
        }

        /// <summary>
        ///     Gets a string enumerator for this <c>Message</c>.
        /// </summary>
        /// <returns>
        ///     A string <c>IEnumerator</c>.
        ///     The enumerator yields the <c>Message</c>'s tag, word,
        ///     and arguments.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
