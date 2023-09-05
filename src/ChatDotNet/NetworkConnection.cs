using System.Data.Common;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Channels;
using ChatDotNet.Enums;
using ChatDotNet.Models;

namespace ChatDotNet;

public class NetworkConnection : IDisposable
{
    public string Logs { get; set; }
    public Server Server { get; private set; }
    public bool Connected => _client.Connected;

    public readonly StreamWriter Writer;
    public readonly StreamReader Reader;

    private readonly NetworkStream _stream;
    private readonly TcpClient _client;
    private readonly int _ping;
    private CancellationTokenSource _source;
    private Task _keepalive;

    public NetworkConnection(Server server, int ping = 15)
    {
        Server = server;

        _client = new TcpClient();
        _client.Connect(server.Hostname, server.Port);
        _stream = _client.GetStream();
        _ping = ping;

        Writer = new StreamWriter(_stream);
        Reader = new StreamReader(_stream);
    }

    public async Task LoginAsync(string nickname, string password = "")
    {
        await Writer.WriteLineAsync($"NICK {nickname}");

        await Writer.WriteLineAsync($"USER {nickname} 0 * :{nickname} Bot");

        if (!string.IsNullOrEmpty(password))
            await Writer.WriteLineAsync($"PRIVMSG NickServ IDENTIFY {password}");

        await Writer.FlushAsync();

        StartKeepAlive();
    }

    public async Task JoinChannelAsync(string to)
    {
        to = to.StartsWith("#") ? to : $"#{to}";

        await SendAsync($"JOIN {to}");
    }

    public async Task LeaveChannelAsync(string from)
    {
        from = from.StartsWith("#") ? from : $"#{from}";

        await SendAsync($"PART {from}");

        _source.Cancel();
    }

    public async Task MessageChannelAsync(string to, string body)
    {
        await SendAsync(to, body);
    }

    public async Task MessageChannelAsync(string to, string[] body)
    {
        await SendAsync(to, body);
    }

    public async Task<(CommandResponse command, string output, string[] parts)> ReadAsync()
    {
        string output = await Reader.ReadLineAsync();

        if (!string.IsNullOrEmpty(output))
        {
            Logs += $"{output}\n";

            string[] parts = output.Split(' ');

            return (GetCommandResponse(parts[1]), output, parts);
        }

        return default;
    }

    public async Task GetChannelNamesAsync(string to)
    {
        to = to.StartsWith("#") ? to : $"#{to}";

        await SendAsync($"NAMES {to}");
    }

    private async Task SendAsync(string command)
    {
        await Writer.WriteLineAsync(command);
        await Writer.FlushAsync();
    }

    private async Task SendAsync(string to, string body)
    {
        to = to.StartsWith("#") ? to : $"#{to}";

        await Writer.WriteLineAsync($"PRIVMSG {to} :{body}");
        await Writer.FlushAsync();
    }

    public async Task SendAsync(string to, string[] body)
    {
        foreach (string line in body)
            await Writer.WriteLineAsync($"PRIVMSG {to} :{line}");

        await Writer.FlushAsync();
    }

    private void StartKeepAlive()
    {
        _source = new CancellationTokenSource();

        var token = _source.Token;

        _keepalive =  new(async () =>
        {

            while (Connected || !token.IsCancellationRequested)
            {
                await PingAsync();

                Thread.Sleep(_ping * 1000);
            }

        }, token);

        _keepalive.Start();
    }

    private static CommandResponse GetCommandResponse(string command)
    {
        CommandResponse commandResponse;

        bool isCommandResponseCode = int.TryParse(command, out int commandResponseCode);

        if (isCommandResponseCode)
        {
            commandResponse = (CommandResponse)commandResponseCode;
        }
        else
        {
            Enum.TryParse(command, out commandResponse);
        }

        return commandResponse;
    }

    private async Task PingAsync()
    {
        await Writer.WriteLineAsync($"PING :{Server.Hostname}");
        await Writer.FlushAsync();
    }

    #region IDisposable
    private bool _disposed;

    private void CloseInstances()
    {
        Writer.Close();
        Reader.Close();
        _stream.Close();
        _client.Close();
        _source.Cancel();

        _disposed = true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                CloseInstances();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
