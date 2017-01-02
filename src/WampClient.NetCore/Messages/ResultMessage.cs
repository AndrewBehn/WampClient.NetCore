using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Result<TSerialization> : IWampPayloadMessage<TSerialization>
    {
        public Result(ulong request, ResultDetails details, Payload<TSerialization> payload)
        {
            Request = request;
            Details = details ?? new ResultDetails();
            Payload = payload;
        }

        public ulong Request { get; }
        public ResultDetails Details { get; }

        public MessageType Type => MessageType.Result;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Details;
            }
        }

        public Payload<TSerialization> Payload { get; }
    }

    public class ResultDetails
    {
    }
}