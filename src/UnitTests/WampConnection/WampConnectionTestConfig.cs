using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WampClient.Core;
using WampClient.Core.Messages;

namespace UnitTests.WampConnection
{
    public class WampConnectTestConfig : TestConfig<WampConnection<string>>
    {
        private readonly List<IWampMessage> _sentMessages = new List<IWampMessage>();
        public WampConnectTestConfig()
        {
            this.Router.Setup(x => x.Messages).Returns(ReceivedMessages);
            this.Router.Setup(x => x.SendMessage(It.IsAny<IWampMessage>(), CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Callback((IWampMessage m, CancellationToken t) =>
                {
                    CallbackImp(m);
                });

            _callBacks.Add(_sentMessages.Add);
        }


        private void CallbackImp(IWampMessage m)
        {
            foreach (var cb in _callBacks)
                cb(m);
        }

        private readonly List<Action<IWampMessage>> _callBacks = new List<Action<IWampMessage>>();

        public void SetupMessageResponse<T, TResponse>(Predicate<T> filter, Func<T, TResponse> responseFactory)
            where T : IWampMessage
            where TResponse : IWampMessage
        {
            _callBacks.Add(m =>
            {
                if (m is T)
                {
                    var t = (T)m;
                    if (filter(t))
                        this.ReceivedMessages.OnNext(responseFactory(t));
                }
            });
        }


        public void SetupMessageResponse<T, TResponse>(Func<T, TResponse> responseFactory)
            where T : IWampMessage
            where TResponse : IWampMessage
        {
            _callBacks.Add(m =>
            {
                if (m is T)
                {
                    var t = (T)m;

                    this.ReceivedMessages.OnNext(responseFactory(t));
                }
            });
        }

        public IEnumerable<IWampMessage> SentMessages => _sentMessages;

        public Subject<IWampMessage> ReceivedMessages = new Subject<IWampMessage>();

        public Mock<IMessageRouter<string>> Router => GetDependency<IMessageRouter<string>>();
    }
}
