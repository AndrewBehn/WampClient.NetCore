using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Unsubscribe : IWampMessage
    {
        public Unsubscribe(ulong request, ulong subscription)
        {
            Request = request;
            Subscription = subscription;
        }

        public ulong Request { get; }
        public ulong Subscription { get; }

        public MessageType Type => MessageType.Unsubscribe;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Subscription;
            }
        }
    }
}