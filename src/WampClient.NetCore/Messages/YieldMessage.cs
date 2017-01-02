using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Yield<TSerialization> : IWampPayloadMessage<TSerialization>
    {
        public Yield(ulong request, YieldOptions options, Payload<TSerialization> payload)
        {
            Request = request;
            Options = options ?? new YieldOptions();
            Payload = payload;
        }

        public ulong Request { get; }
        public YieldOptions Options { get; }

        public MessageType Type => MessageType.Yield;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Options;
            }
        }

        public Payload<TSerialization> Payload { get; }
    }

    public class YieldOptions
    {
    }
}