using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WampClient.Core.Messages;

namespace WampClient.Core
{
    public class MessageRouter<TSerialization> : IMessageRouter<TSerialization>
    {
        private readonly IWebSocket<TSerialization> _socket;
        private readonly IMessageSerializer<TSerialization> _messageSerializer;

        public MessageRouter(IMessageSerializer<TSerialization> messageSerializer, IWebSocket<TSerialization> webSocket)
        {
            _socket = webSocket;
            _messageSerializer = messageSerializer;
            Messages = _socket.Messages.Select(_messageSerializer.Read);

        }

        public IObservable<IWampMessage> Messages { get; }

        public async Task SendMessage(IWampMessage message, CancellationToken token)
        {
            var serialized = _messageSerializer.Write(message);
            await _socket.SendMessage(serialized, token);
        }

        public async Task Connect(string uri, CancellationToken token)
        {
            await _socket.Connect(uri, token);
        }
    }
}
