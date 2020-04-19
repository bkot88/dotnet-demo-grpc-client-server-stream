using Grpc.Core;
using GrpcDemo;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly BlockingQueue<string> _queue;

        public GreeterService(ILogger<GreeterService> logger, BlockingQueue<string> queue)
        {
            _logger = logger;
            _queue = queue;
        }

        public async override Task SayHello(
            HelloRequest request,
            IServerStreamWriter<HelloReply> responseStream,
            ServerCallContext context)
        {
            var t = new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                _queue.Add("hello world");
            });
            t.Start();

            _logger.LogInformation("about to take an item from the queue");
            var item = _queue.Take();

            for (int i = 0; i < 3; i++)
            {
                await responseStream.WriteAsync(new HelloReply
                {
                    Message = $"hello from {i}"
                });
            }
        }
    }
}
