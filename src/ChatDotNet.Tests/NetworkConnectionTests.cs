using ChatDotNet;
using ChatDotNet.Models;
using NUnit.Framework;
using System.Threading.Channels;

namespace ChatDotNet.Tests;

public class NetworkConnectionTests
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
    public void CanConnectToNetwork()
    {
        using NetworkConnection network = new(_server);

        Assert.That(network.Connected, Is.True);
    }

    [Test]
    public async Task CanMessageChannelWithoutJoining()
    {
        using NetworkConnection network = new(_server);

        Assert.That(network.Connected, Is.True);

        await network.LoginAsync(_user.Nickname, _user.Password);

        await network.MessageChannelAsync(_channel, "beep");
    }
}