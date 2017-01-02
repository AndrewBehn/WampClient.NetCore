using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Event<TSerialization> : IWampPayloadMessage<TSerialization>
    {
        public Event(ulong subscription, ulong publication, EventDetails details, Payload<TSerialization> payload)
        {
            Subscription = subscription;
            Publication = publication;
            Details = details ?? new EventDetails();
            Payload = payload;
        }

        public ulong Subscription { get; }
        public ulong Publication { get; }
        public EventDetails Details { get; }

        public MessageType Type => MessageType.Event;
        public Payload<TSerialization> Payload { get; }

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Subscription;
                yield return Publication;
                yield return Details;
            }
        }
    }

    public class EventDetails
    {
    }
}