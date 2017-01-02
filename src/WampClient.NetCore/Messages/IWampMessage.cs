using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public interface IWampMessage
    {
        MessageType Type { get; }

        IEnumerable<object> Components { get; }
    }

    public interface IWampPayloadMessage<TSerialization> : IWampMessage
    {
        Payload<TSerialization> Payload { get; }
    }
}