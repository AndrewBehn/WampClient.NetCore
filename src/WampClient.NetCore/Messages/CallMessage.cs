using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Call<TSerialization> : IWampPayloadMessage<TSerialization>
    {
        public Call(ulong request, CallOptions options, string procedure, Payload<TSerialization> payload)
        {
            Request = request;
            Options = options ?? new CallOptions();
            Procedure = procedure;
            Payload = payload;
        }

        public ulong Request { get; }
        public CallOptions Options { get; }
        public string Procedure { get; }

        public MessageType Type => MessageType.Call;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Options;
                yield return Procedure;
            }
        }

        public Payload<TSerialization> Payload { get; }
    }

    public class CallOptions
    {
    }
}