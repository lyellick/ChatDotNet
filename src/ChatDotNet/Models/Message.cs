using ChatDotNet.Enums;

namespace ChatDotNet.Models
{
    public class Message
    {
        public ListenerHooks Hook { get; set; }

        public string[] Parts { get; set; }

        public string To { get; set; }
        
        public string From { get; set; }
        
        public string Body { get; set; }
    }
}
