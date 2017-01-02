using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WampClient.Core.Exceptions;
using WampClient.Core.Extensions;
using WampClient.Core.Messages;

namespace WampClient.Core
{
    public interface IWampConnection<TSerialization>
    {
        Task<Welcome> Connect(string uri, string realm);
        Task<Subscription<TSerialization>> Subscribe(string topic);
        Task<Unsubscribed> Unsubsribe(ulong subscription);
        Task<Published> Publish(string topic, Payload<TSerialization> payload, bool verify = true);

        Task<Registration> Register(string procedure,
            Func<Payload<TSerialization>, Task<Payload<TSerialization>>> handler);

        Task<Unregistered> Unregister(ulong registration);
        Task<Result<TSerialization>> Call(string procedure, Payload<TSerialization> payload);
    }
 

    public class Registration : IDisposable
    {
        private readonly Action _disposeAction;
        public Registration(Registered registered, Action disposeAction)
        {
            Registered = registered;
            _disposeAction = disposeAction;
        }

        public Registered Registered { get; }

        public void Dispose()
        {
            _disposeAction();
        }
    }

    public class Subscription<TSerialization> : IDisposable
    {
        private readonly Action _disposeAction;
        public Subscription(Subscribed subscribed, IObservable<Event<TSerialization>> relevantEvents, Action disposeAction)
        {
            Subscribed = subscribed;
            RelevantEvents = relevantEvents;
            _disposeAction = disposeAction;
        }

        public Subscribed Subscribed { get; }

        public IObservable<Event<TSerialization>> RelevantEvents { get; }
        public void Dispose()
        {
            _disposeAction();
        }
    }
}