using System.Windows.Input;

namespace ironfrost
{
    /// <summary>
    ///   Custom WPF commands.
    /// </summary>
    class Commands
    {
        public static readonly RoutedUICommand OpenClientWindow =
            new RoutedUICommand("Open Panel", "OpenClientWindow", typeof(Commands));
    }
}
