using Msgpack;
using Msgpack.Extensions;
using Msgpack.Serializer;
using Msgpack.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WampClient.Core.Messages;

namespace WampClient.Core.MsgPackSerialization
{
    public class MsgpackMessageSerializer : IMessageSerializer<byte[]>
    {
        private static readonly MsgpackSerializerSettings Settings = new MsgpackSerializerSettings()
        {
            NameValueHandling = NameValueHandling.Lower
        };

        private static readonly MsgpackSerializer Serializer = new MsgpackSerializer(Settings);

        public IWampMessage Read(byte[] input)
        {
            MArray array = MArray.Parse(input);

            var messageType = array.ConvertIndex<MessageType>(0);

            switch (messageType)
            {
                case MessageType.Abort:
                    return new Abort(array.ConvertIndex<AbortDetails>(1), array.ConvertIndex<string>(2));
                case MessageType.Call:
                    return new Call<byte[]>(array.ConvertIndex<ulong>(1), array.ConvertIndex<CallOptions>(2),
                        array.ConvertIndex<string>(3), array.ConstructPayload(4, 5));
                case MessageType.Event:
                    return new Event<byte[]>(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2),
                        array.ConvertIndex<EventDetails>(3), array.ConstructPayload(4, 5));
                case MessageType.Goodbye:
                    return new Goodbye(array.ConvertIndex<GoodbyeDetails>(1), array.ConvertIndex<string>(2));
                case MessageType.Hello:
                    return new Hello(array.ConvertIndex<string>(1), array.ConvertIndex<HelloDetails>(2));
                case MessageType.Invocation:
                    return new Invocation<byte[]>(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2),
                        array.ConvertIndex<InvocationDetails>(3), array.ConstructPayload(4, 5));
                case MessageType.Publish:
                    return new Publish<byte[]>(array.ConvertIndex<ulong>(1), array.ConvertIndex<PublishOptions>(2),
                        array.ConvertIndex<string>(3), array.ConstructPayload(4, 5));
                case MessageType.Published:
                    return new Published(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Register:
                    return new Register(array.ConvertIndex<ulong>(1), array.ConvertIndex<RegisterOptions>(2),
                        array.ConvertIndex<string>(3));
                case MessageType.Registered:
                    return new Registered(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Result:
                    return new Result<byte[]>(array.ConvertIndex<ulong>(1), array.ConvertIndex<ResultDetails>(2),
                        array.ConstructPayload(3, 4));
                case MessageType.Subscribe:
                    return new Subscribe(array.ConvertIndex<ulong>(1), array.ConvertIndex<SubscribeOptions>(2),
                        array.ConvertIndex<string>(3));
                case MessageType.Subscribed:
                    return new Subscribed(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Unregister:
                    return new Unregister(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Unregistered:
                    return new Unregistered(array.ConvertIndex<ulong>(1));
                case MessageType.Unsubscribe:
                    return new Unsubscribe(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Unsubscribed:
                    return new Unsubscribed(array.ConvertIndex<ulong>(1));
                case MessageType.Welcome:
                    return new Welcome(array.ConvertIndex<ulong>(1), array.ConvertIndex<WelcomeDetails>(2));
                case MessageType.Yield:
                    return new Yield<byte[]>(array.ConvertIndex<ulong>(1), array.ConvertIndex<YieldOptions>(2),
                        array.ConstructPayload(3, 4));
                case MessageType.Error:
                    {
                        var errorMessageType = array.ConvertIndex<MessageType>(1);
                        switch (errorMessageType)
                        {
                            case MessageType.Subscribe:
                                return new SubscribeError(array.ConvertIndex<ulong>(2), array.ConvertIndex<ErrorDetails>(3),
                                    array.ConvertIndex<string>(4));
                            case MessageType.Unsubscribe:
                                return new UnsubscribeError(array.ConvertIndex<ulong>(2), array.ConvertIndex<ErrorDetails>(3),
                                    array.ConvertIndex<string>(4));
                            case MessageType.Publish:
                                return new PublishError(array.ConvertIndex<ulong>(2), array.ConvertIndex<ErrorDetails>(3),
                                    array.ConvertIndex<string>(4));
                            case MessageType.Register:
                                return new RegisterError(array.ConvertIndex<ulong>(2), array.ConvertIndex<ErrorDetails>(3),
                                    array.ConvertIndex<string>(4));
                            case MessageType.Unregister:
                                return new UnregisterError(array.ConvertIndex<ulong>(2), array.ConvertIndex<ErrorDetails>(3),
                                    array.ConvertIndex<string>(4));
                            case MessageType.Invocation:
                                return new InvocationError<byte[]>(array.ConvertIndex<ulong>(2),
                                    array.ConvertIndex<ErrorDetails>(3), array.ConvertIndex<string>(4),
                                    array.ConstructPayload(5, 6));
                            case MessageType.Call:
                                return new CallError(array.ConvertIndex<ulong>(2), array.ConvertIndex<ErrorDetails>(3),
                                    array.ConvertIndex<string>(4));
                            default:
                                throw new InvalidOperationException();
                        }
                    }
                default:
                    throw new InvalidOperationException();
            }


            throw new InvalidOperationException();
        }

        public byte[] Write(IWampMessage message)
        {
            var components = new List<byte[]> { MsgpackConvert.Serialize(message.Type, Settings) };

            var hello = message as Hello;
            var call = message as Call<byte[]>;
            var result = message as Result<byte[]>;
            var register = message as Register;
            var unregister = message as Unregister;
            var yield = message as Yield<byte[]>;
            var payloadMsg = message as IWampPayloadMessage<byte[]>;
            var abort = message as Abort;
            var goodbye = message as Goodbye;
            var publish = message as Publish<byte[]>;
            var subscribe = message as Subscribe;
            var unsubscribe = message as Unsubscribe;
            var invocationError = message as InvocationError<byte[]>;

            if (hello != null)
            {
                components.Add(Convert(hello.Realm));
                components.Add(Convert(hello.Details));
            }
            else if (abort != null)
            {
                components.Add(Convert(abort.Details));
                components.Add(Convert(abort.Reason));
            }
            else if (goodbye != null)
            {
                components.Add(Convert(goodbye.Details));
                components.Add(Convert(goodbye.Reason));
            }
            else if (publish != null)
            {
                components.Add(Convert(publish.Request));
                components.Add(Convert(publish.Options));
                components.Add(Convert(publish.Topic));
            }
            else if (subscribe != null)
            {
                components.Add(Convert(subscribe.Request));
                components.Add(Convert(subscribe.Options));
                components.Add(Convert(subscribe.Topic));
            }
            else if (unsubscribe != null)
            {
                components.Add(Convert(unsubscribe.Request));
                components.Add(Convert(unsubscribe.Subscription));
            }
            else if (call != null)
            {
                components.Add(Convert(call.Request));
                components.Add(Convert(call.Options));
                components.Add(Convert(call.Procedure));
            }
            else if (result != null)
            {
                components.Add(Convert(result.Request));
                components.Add(Convert(result.Details));
            }

            else if (register != null)
            {
                components.Add(Convert(register.Request));
                components.Add(Convert(register.Options));
                components.Add(Convert(register.Procedure));
            }
            else if (unregister != null)
            {
                components.Add(Convert(unregister.Request));
                components.Add(Convert(unregister.Registration));
            }
            else if (yield != null)
            {
                components.Add(Convert(yield.Request));
                components.Add(Convert(yield.Options));
            }
            else if (invocationError != null)
            {
                components.Add(Convert(invocationError.ErrorType));
                components.Add(Convert(invocationError.Request));
                components.Add(Convert(invocationError.Details));
                components.Add(Convert(invocationError.Error));
            }
            else
            {
                throw new InvalidOperationException();
            }


            if (payloadMsg != null)
            {
                var hasArgs = payloadMsg.Payload?.Arguments != null;
                var hasArgKws = payloadMsg.Payload?.ArgumentKeywords != null;

                if (hasArgs || hasArgKws)
                {
                    components.Add(payloadMsg.Payload?.Arguments ?? Enumerable.Empty<byte>().ToArray());
                    if (hasArgKws)
                        components.Add(payloadMsg.Payload.ArgumentKeywords);
                }
            }

            using (var stream = new MemoryStream())
            {
                var writer = new MsgpackStreamWriter(stream);
                writer.WriteArraySize(components.Count);
                foreach (var item in components)
                    stream.WriteAll(item);

                return stream.ToArray();
            }
        }

        public static byte[] Convert<T>(T input)
        {
            return MsgpackConvert.Serialize(input, Settings);
        }
    }

    public static class ConvertExtensions
    {
        public static T ConvertIndex<T>(this MArray input, int index)
        {
            return input.Items[index].ToObject<T>();
        }

        public static MsgpackPayload ConstructPayload(this MArray input, int argIndex, int argKwIndex)
        {
            var count = input.Items.Length;
            var hasArgs = argIndex < count;
            var hasArgKws = argKwIndex < count;

            byte[] args = hasArgs ? input.Items[argIndex].ToBytes() : null;
            byte[] argKws = hasArgKws ? input.Items[argKwIndex].ToBytes() : null;

            return new MsgpackPayload(args, argKws);
        }
    }
}