using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WampClient.Core.Messages
{
    public class Hello : IWampMessage
    {
        public Hello(string realm, HelloDetails details)
        {
            Realm = realm;
            Details = details ?? HelloDetails.Default;
        }

        public string Realm { get; }

        public HelloDetails Details { get; }

        public MessageType Type => MessageType.Hello;

        public IEnumerable<object> Components
        {
            get
            {
                yield return (int)Type;
                yield return Realm;
                yield return Details;
            }
        }
    }

    [DataContract]
    public class HelloDetails
    {
        [DataMember(Name="roles")]
        public ClientRoles Roles { get; } = ClientRoles.Default;

        public static readonly HelloDetails Default = new HelloDetails();
    }

    [DataContract]
    public class ClientRoles
    {
        [DataMember(Name = "publisher")]
        public PublisherDetails Publisher { get; } = PublisherDetails.Default;

        [DataMember(Name = "subscriber")]
        public SubscriberDetails Subscriber { get; } = SubscriberDetails.Default;

        [DataMember(Name = "caller")]
        public CallerDetails Caller { get; } = CallerDetails.Default;

        [DataMember(Name = "callee")]
        public CalleeDetails Callee { get; } = CalleeDetails.Default;

        public static readonly ClientRoles Default = new ClientRoles();
    }

    public class PublisherDetails
    {
        public static readonly PublisherDetails Default = new PublisherDetails();
    }

    public class SubscriberDetails
    {
        public static readonly SubscriberDetails Default = new SubscriberDetails();
    }

    public class CallerDetails
    {
        public static readonly CallerDetails Default = new CallerDetails();
    }

    public class CalleeDetails
    {
        public static readonly CalleeDetails Default = new CalleeDetails();
    }
}