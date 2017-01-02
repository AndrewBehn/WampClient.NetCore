using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WampClient.Core.Messages;

namespace WampClient.Core
{
    public interface IMessageRouter<TSerialization>
    {
        IObservable<IWampMessage> Messages { get; }
        Task SendMessage(IWampMessage message, CancellationToken token);
        Task Connect(string uri, CancellationToken token);
    }
}
