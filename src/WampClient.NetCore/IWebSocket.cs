using Msgpack.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WampClient.Core
{
    public interface IWebSocket<TSerialization>
    {
        IObservable<TSerialization> Messages { get; }
        Task Connect(string uri, CancellationToken token);
        Task SendMessage(TSerialization message, CancellationToken token);
    }

    public class JsonReactiveWebSocket : ReactiveWebSocket<string>
    {
        public JsonReactiveWebSocket() : base("wamp.2.json")
        {
            MessageReceived += (o, e) => Console.WriteLine($"Received: {e.Content}");
            Messages = Observable.FromEventPattern<MessageEventArgs<string>>(eh => MessageReceived += eh,
                    eh => MessageReceived -= eh)
                .Select(m => m.EventArgs.Content);
        }

        public override async Task Connect(string uri, CancellationToken token)
        {
            await _socket.ConnectAsync(new Uri(uri), token);
            _receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[ReceiveChunkSize];
                //try
                //{
                while (_socket.State == WebSocketState.Open)
                {
                    var builder = new StringBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            if (!_closeRequested)
                                throw new Exception();
                            await
                                _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            builder.Append(str);
                        }
                    } while (!result.EndOfMessage);

                    MessageReceived?.Invoke(this, new MessageEventArgs<string>(builder.ToString()));
                }
                //}
            }, token);
        }

        public override IObservable<string> Messages { get; }
        public override async Task SendMessage(string message, CancellationToken token)
        {
            if (_socket.State != WebSocketState.Open)
                throw new InvalidOperationException();

            Console.WriteLine($"Sending {message}");

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messageCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messageCount; i++)
            {
                var offset = SendChunkSize * i;
                var count = SendChunkSize;
                var lastMessage = i + 1 == messageCount;

                if (count * (i + 1) > messageBuffer.Length)
                    count = messageBuffer.Length - offset;

                await
                    _socket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text,
                        lastMessage, token);
            }
        }

        protected event EventHandler<MessageEventArgs<string>> MessageReceived;
    }

    public class MsgpackReactiveWebSocket : ReactiveWebSocket<byte[]>
    {
        public MsgpackReactiveWebSocket() : base("wamp.2.msgpack")
        {
            MessageReceived += (o, e) =>
            {
                using (var s = new MemoryStream(e.Content))
                {
                    var token = MsgpackTokenParser.Instance.ReadToken(s);
                    Console.WriteLine($"Receiving: {token.ToString()}");
                };

            };

            Messages = Observable.FromEventPattern<MessageEventArgs<byte[]>>(eh => MessageReceived += eh,
                    eh => MessageReceived -= eh)
                .Select(m => m.EventArgs.Content);
        }

        public override async Task Connect(string uri, CancellationToken token)
        {
            await _socket.ConnectAsync(new Uri(uri), token);
            _receiveTask = Task.Run(async () =>
            {
                var buffer = new byte[ReceiveChunkSize];
                //try
                //{
                while (_socket.State == WebSocketState.Open)
                {
                    //var builder = new StringBuilder();
                    var builder = new ByteArrayBuilder();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            if (!_closeRequested)
                                throw new Exception();
                            await
                                _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                        }
                        else
                        {
                            //var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            //builder.Append(str);

                            builder.Append(buffer.Take(result.Count).ToArray());
                        }
                    } while (!result.EndOfMessage);

                    MessageReceived?.Invoke(this, new MessageEventArgs<byte[]>(builder.ToArray()));
                }
                //}
            }, token);
        }

        public override IObservable<byte[]> Messages { get; }

        public override async Task SendMessage(byte[] message, CancellationToken cToken)
        {
            if (_socket.State != WebSocketState.Open)
                throw new InvalidOperationException();

            using (var s = new MemoryStream(message))
            {
                var token = MsgpackTokenParser.Instance.ReadToken(s);
                Console.WriteLine($"Receiving: {token.ToString()}");
            };

            var messageCount = (int)Math.Ceiling((double)message.Length / SendChunkSize);

            for (var i = 0; i < messageCount; i++)
            {
                var offset = SendChunkSize * i;
                var count = SendChunkSize;
                var lastMessage = i + 1 == messageCount;

                if (count * (i + 1) > message.Length)
                    count = message.Length - offset;

                await
                    _socket.SendAsync(new ArraySegment<byte>(message, offset, count), WebSocketMessageType.Binary,
                        lastMessage, cToken);
            }
        }

        protected event EventHandler<MessageEventArgs<byte[]>> MessageReceived;
    }

    public class ByteArrayBuilder
    {
        private readonly List<byte[]> _segments = new List<byte[]>();

        public void Append(byte[] array)
        {
            _segments.Add(array);
        }

        public byte[] ToArray()
        {
            return Combine(_segments);
        }

        private byte[] Combine(IEnumerable<byte[]> arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }


    public abstract class ReactiveWebSocket<T> : IWebSocket<T>
    {
        protected const int ReceiveChunkSize = 1024;
        protected const int SendChunkSize = 1024;
        protected readonly ClientWebSocket _socket;

        protected bool _closeRequested;
        protected Task _receiveTask;

        protected ReactiveWebSocket(string subprotocol)
        {
            _socket = new ClientWebSocket();
            _socket.Options.AddSubProtocol(subprotocol);
        }

        public abstract Task Connect(string uri, CancellationToken token);

        public abstract IObservable<T> Messages { get; }

        public abstract Task SendMessage(T message, CancellationToken token);

        protected class MessageEventArgs<TInner> : EventArgs
        {
            public MessageEventArgs(TInner content)
            {
                Content = content;
            }

            public TInner Content { get; }
        }

        public async Task Close()
        {
            _closeRequested = true;
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal", CancellationToken.None);
            if (_receiveTask != null)
                await _receiveTask;
        }
    }
}