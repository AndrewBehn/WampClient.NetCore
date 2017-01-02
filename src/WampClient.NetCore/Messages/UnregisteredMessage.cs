using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Unregistered : IWampMessage
    {
        public Unregistered(ulong request)
        {
            Request = request;
        }

        public ulong Request { get; }

        public MessageType Type => MessageType.Unregistered;

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