using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WampClient.Core.Extensions;
using WampClient.Core.Messages;

namespace WampClient.Core
{
   public static class WampClient
    {
        public static IDisposable Publish<TSerialization>(this IWampConnection<TSerialization> connection, string topic,
            IObservable<Payload<TSerialization>> events)
        {
            return events.SubscribeAsync(async e => await connection.Publish(topic, e));
        }

        public static async Task<IDisposable> Register<TSerialization>(this IWampConnection<TSerialization> connection,
            string procedure, Func<Payload<TSerialization>, Task<Payload<TSerialization>>> handler)
        {
            return await connection.Register(procedure, handler);
        }

        public static IObservable<Payload<TSerialization>> Subscribe<TSerialization>(
            this IWampConnection<TSerialization> connection, string topic)
        {
            return Observable.Create<Payload<TSerialization>>(async o =>
            {
                var subscription = await connection.Subscribe(topic);
                var disp = subscription.RelevantEvents.Select(e => e.Payload).Subscribe(o);

                return Disposable.Create(() =>
                {
                    disp.Dispose();
                    connection.Unsubsribe(subscription.Subscribed.Subscription);
                });
            });
        }

        public static IObservable<Payload<TSerialization>> Call<TSerialization>(
            this IWampConnection<TSerialization> connection, string procedure,
            IObservable<Payload<TSerialization>> events)
        {
            return events.SelectMany(async e =>
            {
                var returnValue = await connection.Call(procedure, e);
                return returnValue.Payload;
            });
        }
    }
}
