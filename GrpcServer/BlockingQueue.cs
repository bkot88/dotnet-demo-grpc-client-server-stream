using System.Collections.Concurrent;

namespace GrpcServer
{
    public class BlockingQueue<T> : BlockingCollection<T> 
    {
        public BlockingQueue() : base(new ConcurrentQueue<T>()) { }
    }
}
