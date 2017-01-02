using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WampClient.Core.Exceptions;
using WampClient.Core.Extensions;
using WampClient.Core.Messages;

namespace WampClient.Core
{
    public class WampConnection<TSerialization> : IWampConnection<TSerialization>
    {
        private readonly WampIdGenerator _idGenerator = new WampIdGenerator();
        private MessageSorter<TSerialization> Sorter => _messageSorterLazy.Value;
        private readonly IMessageRouter<TSerialization> _router;

        public WampConnection(IMessageRouter<TSerialization> router)
        {
            _router = router;
            _messageSorterLazy = new Lazy<MessageSorter<TSerialization>>(() => new MessageSorter<TSerialization>(router));
        }

        private readonly Lazy<MessageSorter<TSerialization>> _messageSorterLazy;

        public async Task<Welcome> Connect(string uri, string realm)
        {
            await _router.Connect(uri, CancellationToken.None);
            var hello = new Hello(realm, null);

            var combined = Sorter.Welcome.Merge<IWampMessage>(Sorter.Abort)
                .Select(o =>
                {
                    if (o.Type == MessageType.Welcome)
                        return (Welcome)o;
                    throw new AbortException((Abort)o);
                })
                .Replay();

            using (combined.Connect())
            {
                await _router.SendMessage(hello, CancellationToken.None);
                return await combined.FirstAsync();
            }
        }

        public async Task<Subscription<TSerialization>> Subscribe(string topic)
        {
            var request = _idGenerator.GenerateId();
            var subscribe = new Subscribe(request, null, topic);

            var subscribeds = Sorter.Subscribed.Where(r => r.Request == request);
            var errors = Sorter.SubscribeError.Where(r => r.Request == request);

            var combined = subscribeds.Merge<IWampMessage>(errors)
                .Select(o =>
                {
                    if (o.Type == MessageType.Subscribed)
                        return (Subscribed)o;
                    throw new SubscribeException((SubscribeError)o);
                })
                .Replay();

            using (combined.Connect())
            {
                await _router.SendMessage(subscribe, CancellationToken.None);
                var eventReplay = Sorter.Event.Replay();
                var connection = eventReplay.Connect();
                var subscribed = await combined.FirstAsync();
                var releventEvents = eventReplay.Where(e => e.Subscription == subscribed.Subscription);
                return new Subscription<TSerialization>(subscribed, releventEvents, () =>
                {
                    connection.Dispose();
                });
            }
        }

        public async Task<Unsubscribed> Unsubsribe(ulong subscription)
        {
            var request = _idGenerator.GenerateId();
            var unsubscribe = new Unsubscribe(request, subscription);

            var unsubscribeds = Sorter.Unsubscribed.Where(r => r.Request == request);
            var errors = Sorter.UnsubscribeError.Where(r => r.Request == request);

            var combined = unsubscribeds.Merge<IWampMessage>(errors)
                .Select(o =>
                {
                    if (o.Type == MessageType.Unsubscribed)
                        return (Unsubscribed)o;
                    throw new UnsubscribeException((UnsubscribeError)o);
                })
                .Replay();

            using (combined.Connect())
            {
                await _router.SendMessage(unsubscribe, CancellationToken.None);
                return await combined.FirstAsync();
            }
        }

        public async Task<Published> Publish(string topic, Payload<TSerialization> payload, bool verify = true)
        {
            var request = _idGenerator.GenerateId();
            var publish = new Publish<TSerialization>(request, verify ? PublishOptions.Verify : PublishOptions.Default,
                topic, payload);

            if (verify)
            {
                var publisheds = Sorter.Published.Where(p => p.Request == request);
                var errors = Sorter.PublishError.Where(p => p.Request == request);

                var combined = publisheds.Merge<IWampMessage>(errors)
                    .Select(o =>
                    {
                        if (o.Type == MessageType.Published)
                            return (Published)o;
                        throw new PublishException((PublishError)o);
                    })
                    .Replay();

                using (combined.Connect())
                {
                    await _router.SendMessage(publish, CancellationToken.None);
                    return await combined.FirstAsync();
                }
            }
            await _router.SendMessage(publish, CancellationToken.None);
            return null;
        }


        public async Task<Registration> Register(string procedure,
            Func<Payload<TSerialization>, Task<Payload<TSerialization>>> handler)
        {
            var request = _idGenerator.GenerateId();
            var register = new Register(request, null, procedure);
            var invocations = Sorter.Invocation.Replay().RefCount();

            var registereds = Sorter.Registered.Where(r => r.Request == request);
            var errors = Sorter.RegisterError.Where(r => r.Request == request);


            var combined = registereds.Merge<IWampMessage>(errors)
                .Select(o =>
                {
                    if (o.Type == MessageType.Registered)
                        return (Registered)o;
                    throw new RegisterException((RegisterError)o);
                })
                .Replay();

            using (combined.Connect())
            {
                await _router.SendMessage(register, CancellationToken.None);
                var registered = await combined.FirstAsync();
                var relevantInvocations = invocations.Where(i => i.Registration == registered.Registration)
                    .SubscribeAsync(async inv =>
                    {
                        try
                        {
                            var returnPayload = await handler(inv.Payload);
                            var yield = new Yield<TSerialization>(inv.Request, null, returnPayload);
                            await _router.SendMessage(yield, CancellationToken.None);
                        }
                        catch (InvocationException e)
                        {
                            var invocationError = new InvocationError<TSerialization>(inv.Request, null, e.Message,
                                inv.Payload);
                            await _router.SendMessage(invocationError, CancellationToken.None);
                        }
                    });
                return new Registration(registered, () => relevantInvocations.Dispose());
            }
        }

        public async Task<Unregistered> Unregister(ulong registration)
        {
            var request = _idGenerator.GenerateId();
            var unregister = new Unregister(request, registration);
            var unregisetereds = Sorter.Unregistered.Where(m => m.Request == request);
            var errors = Sorter.UnregisterError.Where(e => e.Request == request);

            var combined = unregisetereds.Merge<IWampMessage>(errors)
                .Select(o =>
                {
                    if (o.Type == MessageType.Unregistered)
                        return (Unregistered)o;
                    throw new UnregisterException((UnregisterError)o);
                })
                .Replay();

            using (combined.Connect())
            {
                await _router.SendMessage(unregister, CancellationToken.None);
                return await combined.FirstAsync();
            }
        }

        public async Task<Result<TSerialization>> Call(string procedure, Payload<TSerialization> payload)
        {
            var request = _idGenerator.GenerateId();
            var call = new Call<TSerialization>(request, null, procedure, payload);

            var combined = Sorter.Result.Merge<IWampMessage>(Sorter.CallError)
                .Select(o =>
                {
                    if (o.Type == MessageType.Result)
                        return (Result<TSerialization>)o;
                    throw new CallException((CallError)o);
                })
                .Replay();

            using (combined.Connect())
            {
                await _router.SendMessage(call, CancellationToken.None);
                return await combined.FirstAsync();
            }
        }

        public async Task<Goodbye> Disconnect(string reason)
        {
            var goodbye = new Goodbye(null, reason);
            var receivedGoodbyes = Sorter.GoodBye.Replay();
            using (receivedGoodbyes.Connect())
            {
                await _router.SendMessage(goodbye, CancellationToken.None);
                return await Sorter.GoodBye.FirstAsync();
            }
        }
    }
}
