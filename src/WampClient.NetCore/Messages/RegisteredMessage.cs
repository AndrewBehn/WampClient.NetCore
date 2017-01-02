using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Registered : IWampMessage
    {
        public Registered(ulong request, ulong registration)
        {
            Request = request;
            Registration = registration;
        }

        public ulong Request { get; }
        public ulong Registration { get; }

        public MessageType Type => MessageType.Registered;

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