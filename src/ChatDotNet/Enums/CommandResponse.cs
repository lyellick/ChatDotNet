namespace ChatDotNet.Enums
{
    /// <summary>
    /// Response codes returned by server.
    /// </summary>
    public enum CommandResponse
    {
        /// <summary>
        /// Not set enum.
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// No response required message to <channel> or <nickname>.
        /// </summary>
        NOTICE = -1,

        /// <summary>
        /// Private message to <channel> or <nickname>.
        /// </summary>
        PRIVMSG = -2,

        /// <summary>
        /// When a user joins channel <channel>{,<channel>} [<key>{,<key>}].
        /// </summary>
        JOIN = -3,

        /// <summary>
        /// When a user leaves the channel <channel>{,<channel>}.
        /// </summary>
        PART = -4,

        /// <summary>
        /// Server responding to PING.
        /// </summary>
        PONG = -5,

        /// <summary>
        /// Successfully connected to network.
        /// </summary>
        CONNECTED = 1,

        /// <summary>
        /// There are <integer> users and <integer> invisible on <integer> servers
        /// </summary>
        LUSERCLIENT = 251,

        /// <summary>
        /// You have not registered.
        /// </summary>
        NOTREGISTERED = 451,

        /// <summary>
        /// End of /MOTD command.
        /// </summary>
        ENDOFMOTD = 376,

        /// <summary>
        /// MOTD File is missing.
        /// </summary>
        NOMOTD = 422,

        /// <summary>
        /// List of nicknames as <channel> :[[@|+]<nick> [[@|+]<nick> [...]]].
        /// </summary>
        NAMREPLY = 353,

        /// <summary>
        /// End of /NAMES list for <channel>.
        /// </summary>
        ENDOFNAMES = 366,
    }
}
