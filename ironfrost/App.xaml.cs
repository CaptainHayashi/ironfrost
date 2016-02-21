using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
        private BifrostClient bc;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            tok = new Tokeniser();
            try
            {
                bc = new BifrostClient("localhost", 1350, tok);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.Out.WriteLine(ex.ToString());
                throw;
            }

            var wnd = new MainWindow();

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
