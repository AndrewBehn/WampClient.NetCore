using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Unsubscribed : IWampMessage
    {
        public Unsubscribed(ulong request)
        {
            Request = request;
        }

        public ulong Request { get; }

        public MessageType Type => MessageType.Unsubscribed;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
            }
        }
    }
}