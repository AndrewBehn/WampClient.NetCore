using System;
using System.Reactive.Linq;
using WampClient.Core.Messages;

namespace WampClient.Core
{
    public class MessageSorter<TSerialization>
    {
        private readonly IObservable<IWampMessage> _messages;

        public MessageSorter(IMessageRouter<TSerialization> messages)
        {
            _messages = messages.Messages;
        }

        public IObservable<Subscribed> Subscribed => _messages.OfType<Subscribed>();
        public IObservable<Unsubscribed> Unsubscribed => _messages.OfType<Unsubscribed>();
        public IObservable<Published> Published => _messages.OfType<Published>();
        public IObservable<Registered> Registered => _messages.OfType<Registered>();
        public IObservable<Unregistered> Unregistered => _messages.OfType<Unregistered>();
        public IObservable<Result<TSerialization>> Result => _messages.OfType<Result<TSerialization>>();
        public IObservable<Invocation<TSerialization>> Invocation => _messages.OfType<Invocation<TSerialization>>();
        public IObservable<Event<TSerialization>> Event => _messages.OfType<Event<TSerialization>>();
        public IObservable<Welcome> Welcome => _messages.OfType<Welcome>();
        public IObservable<SubscribeError> SubscribeError => _messages.OfType<SubscribeError>();
        public IObservable<UnsubscribeError> UnsubscribeError => _messages.OfType<UnsubscribeError>();
        public IObservable<PublishError> PublishError => _messages.OfType<PublishError>();
        public IObservable<RegisterError> RegisterError => _messages.OfType<RegisterError>();
        public IObservable<UnregisterError> UnregisterError => _messages.OfType<UnregisterError>();

        public IObservable<InvocationError<TSerialization>> InvocationError
            => _messages.OfType<InvocationError<TSerialization>>();

        public IObservable<CallError> CallError => _messages.OfType<CallError>();
        public IObservable<Goodbye> GoodBye => _messages.OfType<Goodbye>();
        public IObservable<Abort> Abort => _messages.OfType<Abort>();
    }
}