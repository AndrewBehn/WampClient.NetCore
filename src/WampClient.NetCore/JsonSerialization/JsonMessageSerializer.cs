using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WampClient.Core.Exceptions;
using WampClient.Core.Messages;
using WampClient.Core.Extensions;

namespace WampClient.Core.JsonSerialization
{
    public class JsonMessageSerializer : IMessageSerializer<string>
    {
        public IWampMessage Read(string input)
        {
            JArray array;
            try
            {
                array = JArray.Parse(input);
            }
            catch (JsonReaderException)
            {
                throw new InvalidMessageException();
            }

            var messageType = array.ConvertIndex<MessageType>(0);

            switch (messageType)
            {
                case MessageType.Abort:
                    return new Abort(array.ConvertIndex<AbortDetails>(1), array.ConvertIndex<string>(2));
                case MessageType.Call:
                    return new Call<string>(array.ConvertIndex<ulong>(1), array.ConvertIndex<CallOptions>(2),
                        array.ConvertIndex<string>(3), array.ConstructPayload(4, 5));
                case MessageType.Event:
                    return new Event<string>(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2),
                        array.ConvertIndex<EventDetails>(3), array.ConstructPayload(4, 5));
                case MessageType.Goodbye:
                    return new Goodbye(array.ConvertIndex<GoodbyeDetails>(1), array.ConvertIndex<string>(2));
                case MessageType.Hello:
                    return new Hello(array.ConvertIndex<string>(1), array.ConvertIndex<HelloDetails>(2));
                case MessageType.Invocation:
                    return new Invocation<string>(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2),
                        array.ConvertIndex<InvocationDetails>(3), array.ConstructPayload(4, 5));
                case MessageType.Publish:
                    return new Publish<string>(array.ConvertIndex<ulong>(1), array.ConvertIndex<PublishOptions>(2),
                        array.ConvertIndex<string>(3), array.ConstructPayload(4, 5));
                case MessageType.Published:
                    return new Published(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Register:
                    return new Register(array.ConvertIndex<ulong>(1), array.ConvertIndex<RegisterOptions>(2),
                        array.ConvertIndex<string>(3));
                case MessageType.Registered:
                    return new Registered(array.ConvertIndex<ulong>(1), array.ConvertIndex<ulong>(2));
                case MessageType.Result:
                    return new Result<string>(array.ConvertIndex<ulong>(1), array.ConvertIndex<ResultDetails>(2),
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
                    return new Yield<string>(array.ConvertIndex<ulong>(1), array.ConvertIndex<YieldOptions>(2),
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
                                return new InvocationError<string>(array.ConvertIndex<ulong>(2),
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
        }

        public string Write(IWampMessage message)
        {
            var array = message.Components.Select(c => JsonConvert.SerializeObject(c, Settings)).ToList();

            var payloadMsg = message as IWampPayloadMessage<string>;
            if (payloadMsg != null)
            {
                var hasArgs = payloadMsg.Payload?.Arguments != null;
                var hasArgKws = payloadMsg.Payload?.ArgumentKeywords != null;

                if (hasArgs)
                {
                    if (!payloadMsg.Payload.Arguments.IsValidJArray())
                        throw new InvalidArgumentsException();
                }
                if (hasArgKws)
                {
                    if (!payloadMsg.Payload.ArgumentKeywords.IsValidJObject())
                        throw new InvalidArgumentKeywordsException();
                }



                if (hasArgs || hasArgKws)
                {
                    array.Add(payloadMsg.Payload?.Arguments ?? "[]");
                    if (hasArgKws)
                        array.Add(payloadMsg.Payload.ArgumentKeywords);
                }
            }
            return $"[{string.Join(",", array)}]";
        }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };
    }

    public static class ConvertExtensions
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public static T ConvertIndex<T>(this JArray input, int index)
        {
            return input[index].ToObject<T>(Serializer);
        }

        public static JsonPayload ConstructPayload(this JArray input, int argIndex, int argKwIndex)
        {
            var count = input.Count;
            var args = argIndex < count ? input[argIndex].ToString(Formatting.None) : null;
            var argKws = argKwIndex < count ? input[argKwIndex].ToString(Formatting.None) : null;

            return new JsonPayload(args, argKws);
        }
    }
}
