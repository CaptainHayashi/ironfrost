namespace Ironfrost.Models
{
    public class Studio
    {
        public string Name { get; }
        public string Host { get; }
        public int Port {  get; }

        public Studio(string name, string host, int port)
        {
            Name = name;
            Host = host;
            Port = port;
        }

        public Studio()
        {
            Name = "(unknown)";
            Host = "localhost";
            Port = 1350;
        }

    }
}