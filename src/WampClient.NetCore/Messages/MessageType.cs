﻿namespace WampClient.Core.Messages
{
    public enum MessageType
    {
        Hello = 1,
        Welcome = 2,
        Abort = 3,
        Goodbye = 6,
        Error = 8,
        Publish = 16,
        Published = 17,
        Subscribe = 32,
        Subscribed = 33,
        Unsubscribe = 34,
        Unsubscribed = 35,
        Event = 36,
        Call = 48,
        Result = 50,
        Register = 64,
        Registered = 65,
        Unregister = 66,
        Unregistered = 67,
        Invocation = 68,
        Yield = 70
    }
}