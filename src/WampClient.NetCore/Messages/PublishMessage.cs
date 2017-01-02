using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WampClient.Core.Messages
{
    public class Publish<TSerialization> : IWampPayloadMessage<TSerialization>
    {
        public Publish(ulong request, PublishOptions options, string topic, Payload<TSerialization> payload)
        {
            Request = request;
            Options = options ?? PublishOptions.Default;
            Topic = topic;
            Payload = payload;
        }

        public ulong Request { get; }
        public PublishOptions Options { get; }
        public string Topic { get; }

        public MessageType Type => MessageType.Publish;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Options;
                yield return Topic;
            }
        }

        public Payload<TSerialization> Payload { get; }
    }

    [DataContract]
    public class PublishOptions
    {
        public static PublishOptions Default = new PublishOptions();

        public static PublishOptions Verify = new PublishOptions
        {
            Acknowledge = true
        };

        [DataMember(Name = "acknowledge")]
        public bool? Acknowledge { get; set; }
    }
}