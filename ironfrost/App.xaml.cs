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
        private ClientSocket bc;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            tok = new Tokeniser();
            try
            {
                bc = new ClientSocket("localhost", 1350, tok);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Console.Out.WriteLine(ex.ToString());
                throw;
            }
            
            var rl = new InitialClientRole();
            
            var client = new Client(bc, rl);

            var wnd = new ClientWindow(bc.Name, rl);

            Task.Run(client.RunAsync);

            wnd.Show();
        }
    }
}
