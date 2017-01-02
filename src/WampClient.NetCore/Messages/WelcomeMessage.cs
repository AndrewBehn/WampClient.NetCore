using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Welcome : IWampMessage
    {
        public Welcome(ulong session, WelcomeDetails details)
        {
            Session = session;
            Details = details ?? new WelcomeDetails();
        }

        public ulong Session { get; }
        public WelcomeDetails Details { get; }

        public MessageType Type => MessageType.Welcome;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Session;
                yield return Details;
            }
        }
    }

    public class WelcomeDetails
    {
    }
}