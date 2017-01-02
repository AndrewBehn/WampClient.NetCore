using System.Collections.Generic;

namespace WampClient.Core.Messages
{
    public class Register : IWampMessage
    {
        public Register(ulong request, RegisterOptions options, string procedure)
        {
            Request = request;
            Options = options ?? new RegisterOptions();
            Procedure = procedure;
        }

        public ulong Request { get; }
        public RegisterOptions Options { get; }
        public string Procedure { get; }

        public MessageType Type => MessageType.Register;

        public IEnumerable<object> Components
        {
            get
            {
                yield return Type;
                yield return Request;
                yield return Options;
                yield return Procedure;
            }
        }
    }

    public class RegisterOptions
    {
    }
}