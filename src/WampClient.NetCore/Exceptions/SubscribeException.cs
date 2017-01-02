using WampClient.Core.Messages;

namespace WampClient.Core.Exceptions
{
    public class SubscribeException : WampErrorException
    {
        public SubscribeException(SubscribeError error) : base(error)
        {
        }
    }

    public class UnsubscribeException : WampErrorException
    {
        public UnsubscribeException(UnsubscribeError error) : base(error)
        {
        }
    }

    public class PublishException : WampErrorException
    {
        public PublishException(PublishError error) : base(error)
        {
        }
    }

    public class RegisterException : WampErrorException
    {
        public RegisterException(RegisterError error) : base(error)
        {
        }
    }

    public class UnregisterException : WampErrorException
    {
        public UnregisterException(UnregisterError error) : base(error)
        {
        }
    }

    public class CallException : WampErrorException
    {
        public CallException(CallError error) : base(error)
        {
        }
    }

    public class InvocationException : WampException
    {
        public InvocationException(string message) : base(message)
        {
        }
    }

    public class AbortException : WampException
    {
        public AbortException(Abort abort) : base(string.Empty)
        {
            AbortMessage = abort;
        }

        public Abort AbortMessage { get; }
    }
}