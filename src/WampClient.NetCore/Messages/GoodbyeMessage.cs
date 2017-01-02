using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Goodbye : IWampMessage
    {
        public Goodbye(GoodbyeDetails details, string reason)
        {
            Details = details ?? new GoodbyeDetails();
            Reason = reason;
        }

        public GoodbyeDetails Details { get; }
        public string Reason { get; }

        public MessageType Type => MessageType.Goodbye;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Details;
                yield return Reason;
            }
        }
    }

    public class GoodbyeDetails
    {
    }
}