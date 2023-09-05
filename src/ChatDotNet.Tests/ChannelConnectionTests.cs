using ChatDotNet;
using ChatDotNet.Models;
using NUnit.Framework;

namespace ChatDotNet.Tests;

public class ChannelConnectionTests
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
    public async Task CanLoginToNetwork()
    {
        using NetworkConnection network = new(_server);

        Assert.That(network.Connected, Is.True);

        ChannelConnection channel = new(network, _channel, _user);

        await channel.LoginAsync();
    }

    [Test]
    public async Task CanJoinChannel()
    {
        using NetworkConnection network = new(_server);

        Assert.That(network.Connected, Is.True);

        ChannelConnection channel = new(network, _channel, _user);

        await channel.LoginAsync();

        await channel.JoinAsync();

        await channel.LeaveAsync();
    }

    [Test]
    public async Task CanMessageChannel()
    {
        using NetworkConnection network = new(_server);

        Assert.That(network.Connected, Is.True);

        ChannelConnection channel = new(network, _channel, _user);

        await channel.LoginAsync();

        await channel.JoinAsync();

        await channel.MessageAsync("beep");

        await channel.LeaveAsync();
    }
}