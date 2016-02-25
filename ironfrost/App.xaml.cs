using System;
using System.Threading.Tasks;
using System.Windows;

namespace ironfrost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Tokeniser tok;
        private Client bc;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            tok = new Tokeniser();
            try
            {
                bc = new Client("localhost", 1350, tok);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.Out.WriteLine(ex.ToString());
                throw;
            }

            var wnd = new ClientWindow(bc.Name, bc.WriteAsync);

            Func<Task> loop = async () =>
            {
                while (true)
                {
                    var lines = await bc.ReadAsync();
                    foreach (var line in lines)
                    {
                        var msg = new Message(line);
                        wnd.Dispatcher.Invoke(() => { wnd.NewMessage(msg); });
                    }
                }
            };
            Task.Run(loop);

            wnd.Show();
        }
    }
}
