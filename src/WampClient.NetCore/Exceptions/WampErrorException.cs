using WampClient.Core.Messages;

namespace WampClient.Core.Exceptions
{
    public abstract class WampErrorException : WampException
    {
        protected WampErrorException(IWampErrorMessage error) : base(string.Empty)
        {
            ErrorType = error.ErrorType;
        }

        public MessageType ErrorType { get; }
    }
}