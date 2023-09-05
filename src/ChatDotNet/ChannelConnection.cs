using ChatDotNet.Enums;
using ChatDotNet.Models;
using System.Data.Common;

namespace ChatDotNet;

public class ChannelConnection
{
    private string[] _members = null;

    private readonly NetworkConnection _network;
    private readonly string _channel;
    private readonly User _user;

    public ChannelConnection(NetworkConnection network, string channel, User user)
    {
        if (!network.Connected)
            throw new Exception("Unable to connect to network.");

        if (string.IsNullOrEmpty(channel))
            throw new ArgumentException("Channel can not be null or empty.");

        _channel = channel;

        _network = network;

        _user = user;
    }

    public async Task LoginAsync()
    {
        await _network.LoginAsync(_user.Nickname, _user.Password);
    }

    public async Task JoinAsync(string body = null)
    {
        await _network.JoinChannelAsync(_channel);
    }

    public async Task LeaveAsync()
    {
        await _network.LeaveChannelAsync(_channel);
    }

    public async Task MessageAsync(string body)
    {
        await _network.MessageChannelAsync(_channel, body);
    }

    public async Task MessageAsync(string[] body)
    {
        await _network.MessageChannelAsync(_channel, body);
    }

    public async Task GetMembersAsync()
    {
        await _network.GetChannelNamesAsync(_channel);
    }
}
