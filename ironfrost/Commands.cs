using System.Windows.Input;

namespace Ironfrost
{
    /// <summary>
    ///   Custom WPF commands.
    /// </summary>
    class Commands
    {
        public static readonly RoutedUICommand OpenConsole =
            new RoutedUICommand("Open Console", "OpenConsole", typeof(Commands));

        public static readonly RoutedUICommand OpenInspector =
            new RoutedUICommand("Open Inspector", "OpenInspector", typeof(Commands));
    }
}
