using System;

namespace WampClient.Core.Exceptions
{
    public abstract class WampException : System.Exception
    {
        protected WampException(string message) : base(message)
        {
        }
    }
}