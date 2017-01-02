using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Published : IWampMessage
    {
        public Published(ulong request, ulong publication)
        {
            Request = request;
            Publication = publication;
        }

        public ulong Request { get; }
        public ulong Publication { get; }

        public MessageType Type => MessageType.Published;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Publication;
            }
        }
    }
}