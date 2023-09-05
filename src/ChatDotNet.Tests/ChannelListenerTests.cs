using ChatDotNet;
using ChatDotNet.Models;
using NUnit.Framework;

namespace ChatDotNet.Tests;

public class ChannelListenerTests
{
    private Server _server;

    private string _channel;

    private User _user;

    [SetUp]
    public void Setup()
    {
        _server = new("irc.fractals.chat");

        _channel = "ChatDotNetTests";

        _user = new()
        {
            Nickname = "fluxbot"
        };
    }

    [Test]
    public async Task CanRunListener()
    {
        CancellationTokenSource source = new CancellationTokenSource();

        using NetworkConnection network = new(_server);

        Assert.That(network.Connected, Is.True);

        ChannelConnection channel = new(network, _channel, _user);

        ChannelListener listener = new(network, channel, source.Token);

        listener.Initialize();

        while (listener.IsListening)
        {
            await Task.Delay(5000);

            await listener.CloseAsync();
        }
    }
}