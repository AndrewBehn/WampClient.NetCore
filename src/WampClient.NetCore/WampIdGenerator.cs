using System.Threading;

namespace WampClient.Core
{
    public class WampIdGenerator
    {
        private long _counter = 1;

        public ulong GenerateId()
        {
            return (ulong)Interlocked.Increment(ref _counter);
        }
    }
}