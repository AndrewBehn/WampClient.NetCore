namespace WampClient.Core.Messages
{
    public abstract class Payload<TSerialization>
    {
        //public static Payload<TSerialization> Empty = new Payload<TSerialization>(default(TSerialization), default(TSerialization));

        protected Payload(TSerialization args, TSerialization argKws)
        {
            Arguments = args;
            ArgumentKeywords = argKws;
        }

        public TSerialization Arguments { get; }
        public TSerialization ArgumentKeywords { get; }
    }
}