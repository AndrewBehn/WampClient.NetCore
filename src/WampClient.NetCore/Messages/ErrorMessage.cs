using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public interface IWampErrorMessage : IWampMessage
    {
        MessageType ErrorType { get; }
    }


    public abstract class ErrorMessage : IWampErrorMessage
    {
        protected ErrorMessage(ulong request, ErrorDetails details, string error)
        {
            Request = request;
            Details = details ?? new ErrorDetails();
            Error = error;
        }

        public ulong Request { get; }
        public ErrorDetails Details { get; }
        public string Error { get; }

        public MessageType Type => MessageType.Error;
        public abstract MessageType ErrorType { get; }

        public virtual IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return ErrorType;
                yield return Request;
                yield return Details;
                yield return Error;
            }
        }
    }

    public class ErrorDetails
    {
    }

    public class SubscribeError : ErrorMessage
    {
        public SubscribeError(ulong request, ErrorDetails details, string error) : base(request, details, error)
        {
        }

        public override MessageType ErrorType => MessageType.Subscribe;
    }

    public class UnsubscribeError : ErrorMessage
    {
        public UnsubscribeError(ulong request, ErrorDetails details, string error) : base(request, details, error)
        {
        }

        public override MessageType ErrorType => MessageType.Unsubscribe;
    }

    public class PublishError : ErrorMessage
    {
        public PublishError(ulong request, ErrorDetails details, string error) : base(request, details, error)
        {
        }

        public override MessageType ErrorType => MessageType.Publish;
    }

    public class RegisterError : ErrorMessage
    {
        public RegisterError(ulong request, ErrorDetails details, string error) : base(request, details, error)
        {
        }

        public override MessageType ErrorType => MessageType.Register;
    }

    public class UnregisterError : ErrorMessage
    {
        public UnregisterError(ulong request, ErrorDetails details, string error) : base(request, details, error)
        {
        }

        public override MessageType ErrorType => MessageType.Unregister;
    }

    public class InvocationError<TSerialization> : ErrorMessage, IWampPayloadMessage<TSerialization>
    {
        public InvocationError(ulong request, ErrorDetails details, string error, Payload<TSerialization> payload)
            : base(request, details, error)
        {
            Payload = payload;
        }

        public override MessageType ErrorType => MessageType.Invocation;

        public Payload<TSerialization> Payload { get; }
    }

    public class CallError : ErrorMessage
    {
        public CallError(ulong request, ErrorDetails details, string error) : base(request, details, error)
        {
        }

        public override MessageType ErrorType => MessageType.Call;
    }
}