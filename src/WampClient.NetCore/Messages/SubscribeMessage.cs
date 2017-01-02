using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Subscribe : IWampMessage
    {
        public Subscribe(ulong request, SubscribeOptions options, string topic)
        {
            Request = request;
            Options = options ?? new SubscribeOptions();
            Topic = topic;
        }

        public ulong Request { get; }
        public SubscribeOptions Options { get; }
        public string Topic { get; }

        public MessageType Type => MessageType.Subscribe;

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
    }

    public class SubscribeOptions
    {
    }
}