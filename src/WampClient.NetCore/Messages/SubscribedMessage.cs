﻿using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Subscribed : IWampMessage
    {
        public Subscribed(ulong request, ulong subscription)
        {
            Request = request;
            Subscription = subscription;
        }

        public ulong Request { get; }
        public ulong Subscription { get; }

        public MessageType Type => MessageType.Subscribed;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Subscription;
            }
        }
    }
}