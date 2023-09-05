using ChatDotNet.Enums;
using ChatDotNet.Extensions;
using ChatDotNet.Models;
using System.Threading.Channels;

namespace ChatDotNet;

public class ChannelListener
{
    public bool IsListening;

    private List<Action<Message, ChannelConnection>> _listeners = new();
    private string[] _members;
    private CancellationToken _token;

    private readonly DateTimeOffset _started;
    private readonly NetworkConnection _network;
    private readonly ChannelConnection _channel;
    private readonly string _manager;

    public ChannelListener(NetworkConnection network, ChannelConnection channel, CancellationToken token, string manager = "")
    {
        _token = token;
        _network = network;
        _channel = channel;
        _manager = manager;
        _started = DateTimeOffset.Now;

        #region Bult-in Listseners
        // Listener: Test
        //      beep
        _listeners.Add((message, channel) =>
        {
            if (message.Hook == ListenerHooks.BEEP)
            {
                Task task = new(async () =>
                {
                    await channel.MessageAsync("boop");
                }, _token);

                task.Start();
            }
        });

        // Listener: Banish
        //      banish
        _listeners.Add((message, channel) =>
        {
            if (message.Hook == ListenerHooks.BANISH)
            {
                Task task = new(async () =>
                {
                    if (string.IsNullOrEmpty(_manager) || message.From.ToLower() == _manager.ToLower())
                    {
                        await _channel.MessageAsync("Goodbye!");
                        await CloseAsync();
                    }
                }, _token);

                task.Start();
            }
        });

        // Listener: Uptime
        //      uptime
        _listeners.Add((message, channel) =>
        {
            if (message.Hook == ListenerHooks.UPTIME)
            {
                Task task = new(async () =>
                {
                    await channel.MessageAsync($"I have been active for {_started.AsTimeAgo()}.");
                }, _token);

                task.Start();
            }
        });

        // Listener: Reminder
        //      remind <seconds> <message>
        _listeners.Add((message, channel) =>
        {
            bool isReminder = message.Hook == ListenerHooks.REMIND;

            if (isReminder && message.Parts.Length > 4)
            {
                bool isValidTimespan = int.TryParse(message.Parts[4], out int seconds);

                if (isValidTimespan)
                {
                    string reminder = string.Join(" ", message.Parts.Skip(5));

                    Task task = new(async () =>
                    {
                        Thread.Sleep(seconds * 1000);
                        await channel.MessageAsync($"~ Reminder: {reminder} ~");
                    });

                    task.Start();
                }
            }
        });
        #endregion
    }

    public void Initialize()
    {
        IsListening = true;

        Task listener = new(async () =>
        {
            await _channel.LoginAsync();

            while (_network.Connected || !_token.IsCancellationRequested || IsListening)
            {
                try
                {
                    var (command, output, parts) = await _network.ReadAsync();

                    switch (command)
                    {
                        case CommandResponse.NOTICE:
                            break;
                        case CommandResponse.NOTREGISTERED:
                            break;
                        case CommandResponse.NOMOTD:
                        case CommandResponse.ENDOFMOTD:
                            await _channel.JoinAsync();
                            break;
                        case CommandResponse.NAMREPLY:
                            _members = output.Split("=")[1].Split(":")[1].Split(" ").Select(member => member.Replace("@", "")).ToArray();
                            break;
                        case CommandResponse.PART:
                        case CommandResponse.JOIN:
                            await _channel.GetMembersAsync();
                            break;
                        case CommandResponse.PRIVMSG:
                            if (parts.Length > 2)
                            {
                                string to = parts[2];
                                string body = output.Split(':')[2];
                                string from = output.Split('!')[0][1..];
                                bool isValidHook = Enum.TryParse(body.Split(' ')[0].ToUpper(), out ListenerHooks hook);

                                if (isValidHook)
                                {
                                    Message message = new() { Hook = hook, Parts = parts, To = to, From = from, Body = body };

                                    foreach (var listener in _listeners)
                                        listener(message, _channel);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    IsListening = false;

                    await _channel.LeaveAsync();
                }
            }

            IsListening = false;

            await _channel.LeaveAsync();
        }, _token);

        listener.Start();
    }

    public async Task CloseAsync()
    {
        await _channel.LeaveAsync();
        IsListening = false;
    }

    public void AddListener(Action<Message, ChannelConnection> listener) => _listeners.Add(listener);

    public string[] GetMembers() => _members;
}
