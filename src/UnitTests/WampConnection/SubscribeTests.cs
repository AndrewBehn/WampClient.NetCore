using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WampClient.Core.Exceptions;
using WampClient.Core.Messages;
using Xunit;

namespace UnitTests.WampConnection
{
    public class SubscribeTests
    {

        [Fact]
        public async Task SubscribeWithImmediateReturn()
        {
            var config = new WampConnectTestConfig();
            ulong subscribeRequestId = 0;

            config.SetupMessageResponse<Subscribe, Subscribed>(_ => true, s =>
            {
                subscribeRequestId = s.Request;
                return new Subscribed(s.Request, 1);
            });

            var subscription = await config.Subject.Subscribe("topic");

            Assert.NotNull(subscription);
            Assert.Equal(subscribeRequestId, subscription.Subscribed.Request);
            Assert.Equal(1u, subscription.Subscribed.Subscription);

            config.ReceivedMessages.OnNext(new Event<string>(subscription.Subscribed.Subscription, 1, null, null));
            config.ReceivedMessages.OnNext(new Event<string>(subscription.Subscribed.Subscription, 2, null, null));

            var events = new List<Event<string>>();
            subscription.RelevantEvents.Subscribe(events.Add);

            Assert.Equal(2, events.Count());
            Assert.True(events.All(e => e.Subscription == subscription.Subscribed.Subscription));
            Assert.True(events.All(e => e.Payload == null));
        }

        [Fact]
        public async Task SubscribeWithDelayedReturn()
        {
            var config = new WampConnectTestConfig();

            var subscribeTask = config.Subject.Subscribe("topic");

            var subscribe = config.SentMessages.OfType<Subscribe>().Single();
            config.ReceivedMessages.OnNext(new Subscribed(subscribe.Request, 1));
            var subscription = await subscribeTask;

            Assert.NotNull(subscription);
            Assert.Equal(subscribe.Request, subscription.Subscribed.Request);
            Assert.Equal(1u, subscription.Subscribed.Subscription);

            config.ReceivedMessages.OnNext(new Event<string>(subscription.Subscribed.Subscription, 1, null, null));
            config.ReceivedMessages.OnNext(new Event<string>(subscription.Subscribed.Subscription, 2, null, null));
            config.ReceivedMessages.OnNext(new Event<string>(subscription.Subscribed.Subscription + 1, 3, null, null));

            var events = new List<Event<string>>();
            subscription.RelevantEvents.Subscribe(events.Add);

            Assert.Equal(2, events.Count());
            Assert.True(events.All(e => e.Subscription == subscription.Subscribed.Subscription));
            Assert.True(events.All(e => e.Payload == null));
        }

        [Fact]
        public async Task SubscribeWithImmediateError()
        {
            var config = new WampConnectTestConfig();

            config.SetupMessageResponse<Subscribe, SubscribeError>(_ => true,
                s => new SubscribeError(s.Request, null, "ruhroh"));

            var exception =
                await Assert.ThrowsAsync<SubscribeException>(async () => await config.Subject.Subscribe("topic"));
            Assert.Equal(MessageType.Subscribe, exception.ErrorType);
        }

        [Fact]
        public async Task SubscribeWithDelayedError()
        {
            var config = new WampConnectTestConfig();

            var subscribeTask = config.Subject.Subscribe("topic");
            var subscribe = config.SentMessages.OfType<Subscribe>().Single();
            config.ReceivedMessages.OnNext(new SubscribeError(subscribe.Request, null, "ruhroh"));

            var exception = await Assert.ThrowsAsync<SubscribeException>(() => subscribeTask);
            Assert.Equal(MessageType.Subscribe, exception.ErrorType);
        }

        [Fact]
        public async Task SubscribeDiscriminatesIncomingSubscribeErrorMessages()
        {
            var config = new WampConnectTestConfig();

            config.SetupMessageResponse<Subscribe, SubscribeError>(_ => true,
                s => new SubscribeError(s.Request + 1, null, "ruhroh"));

            config.SetupMessageResponse<Subscribe, Subscribed>(_ => true,
                s => new Subscribed(s.Request, 1));

            var subscription = await config.Subject.Subscribe("topic");
            Assert.NotNull(subscription);
        }

        [Fact]
        public async Task SubscribeDiscriminatesIncomingSubscribedMessages()
        {
            var config = new WampConnectTestConfig();

            config.SetupMessageResponse<Subscribe, Subscribed>(_ => true,
                s => new Subscribed(s.Request + 1, 1));

            config.SetupMessageResponse<Subscribe, SubscribeError>(_ => true,
                s => new SubscribeError(s.Request, null, "ruhroh"));

            await Assert.ThrowsAsync<SubscribeException>(() => config.Subject.Subscribe("topic"));
        }
    }
}