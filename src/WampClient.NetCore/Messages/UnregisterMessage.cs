using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Unregister : IWampMessage
    {
        public Unregister(ulong request, ulong registration)
        {
            Request = request;
            Registration = registration;
        }

        public ulong Request { get; }
        public ulong Registration { get; }

        public MessageType Type => MessageType.Unregister;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Registration;
            }
        }
    }
}