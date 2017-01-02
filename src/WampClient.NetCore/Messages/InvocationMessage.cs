using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Invocation<TSerialization> : IWampPayloadMessage<TSerialization>
    {
        public Invocation(ulong request, ulong registration, InvocationDetails details, Payload<TSerialization> payload)
        {
            Request = request;
            Registration = registration;
            Details = details ?? new InvocationDetails();
            Payload = payload;
        }

        public ulong Request { get; }
        public ulong Registration { get; }
        public InvocationDetails Details { get; }

        public MessageType Type => MessageType.Invocation;
        public Payload<TSerialization> Payload { get; }

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Registration;
                yield return Details;
            }
        }
    }

    public class InvocationDetails
    {
    }
}