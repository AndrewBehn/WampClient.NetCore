using System;
using Newtonsoft.Json;
using WampClient.Core.Messages;

namespace WampClient.Core
{
    public interface IMessageSerializer<TSerialization>
    {
        TSerialization Write(IWampMessage message);
        IWampMessage Read(TSerialization input);
    }
}