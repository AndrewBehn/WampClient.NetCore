using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Abort : IWampMessage
    {
        public Abort(AbortDetails details, string reason)
        {
            Details = details ?? new AbortDetails();
            Reason = reason;
        }

        public AbortDetails Details { get; }
        public string Reason { get; }

        public MessageType Type => MessageType.Abort;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Details;
                yield return Reason;
            }
        }
    }

    public class AbortDetails
    {
    }
}