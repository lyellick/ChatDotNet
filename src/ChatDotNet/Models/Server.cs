namespace ChatDotNet.Models;

public class Server
{
    public Server() { }

    public Server(string hostname, int port = 6667)
    {
        Hostname = hostname;
        Port = port;
    }

    public string Hostname { get; set; }

    public int Port { get; set; }
}
