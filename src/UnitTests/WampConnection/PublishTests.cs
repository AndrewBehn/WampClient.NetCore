using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using WampClient.Core.Exceptions;
using WampClient.Core.Messages;
using Xunit;

namespace UnitTests.WampConnection
{
    public class PublishTests
    {
        [Fact]
        public async Task PublishWithImmediateReturn()
        {
            var config = new WampConnectTestConfig();
            ulong publishRequestId = 0;

            config.SetupMessageResponse<Publish<string>, Published>(_ => true, s =>
            {
                publishRequestId = s.Request;
                return new Published(s.Request, 1);
            });

            var published = await config.Subject.Publish("topic", null);

            Assert.NotNull(published);
            Assert.Equal(publishRequestId, published.Request);
        }

        [Fact]
        public async Task PublishWithDelayedReturn()
        {
            var config = new WampConnectTestConfig();

            var publishTask = config.Subject.Publish("topic", null);

            var publish = config.SentMessages.OfType<Publish<string>>().Single();
            config.ReceivedMessages.OnNext(new Published(publish.Request, 1));
            var published = await publishTask;

            Assert.NotNull(published);
            Assert.Equal(publish.Request, published.Request);
        }

        [Fact]
        public async Task PublishWithImmediateError()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Publish<string>, PublishError>(_ => true,
                p => new PublishError(p.Request, null, "ruhroh"));

            var publishException = await Assert.ThrowsAsync<PublishException>(() => config.Subject.Publish("topic", null));
            Assert.NotNull(publishException);
        }

        [Fact]
        public async Task PublishWithDelayedError()
        {
            var config = new WampConnectTestConfig();
            var publishTask = config.Subject.Publish("topic", null);

            var publish = config.SentMessages.OfType<Publish<string>>().Single();
            config.ReceivedMessages.OnNext(new PublishError(publish.Request, null, "ruhroh"));

            var exception = await Assert.ThrowsAsync<PublishException>(() => publishTask);
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task PublishWitoutAcknowledgement()
        {
            var config = new WampConnectTestConfig();
            var published = await config.Subject.Publish("topic", null, false);
            Assert.Null(published);
            Assert.NotNull(config.SentMessages.OfType<Publish<string>>().Single());
        }

        [Fact]
        public async Task PublishDiscriminatesIncomingPublishedMessages()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Publish<string>, Published>(p => new Published(p.Request + 1, 1));
            config.SetupMessageResponse<Publish<string>, PublishError>(p => new PublishError(p.Request, null, "ruhroh"));

            await Assert.ThrowsAsync<PublishException>(() => config.Subject.Publish("topic", null));
        }


        [Fact]
        public async Task PublishDiscriminatesIncomingPublishErrorMessage()
        {
            var config = new WampConnectTestConfig();
            config.SetupMessageResponse<Publish<string>, PublishError>(p => new PublishError(p.Request + 1, null, "ruhroh"));
            config.SetupMessageResponse<Publish<string>, Published>(p => new Published(p.Request, 1));

            var published = await config.Subject.Publish("topic", null);
            Assert.NotNull(published);
        }
    }
}
