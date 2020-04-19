using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemo;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GrpcClient
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch(
                "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var options = new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure
            };
            using var channel = GrpcChannel.ForAddress("http://localhost:5000", options);
            var client = new GrpcDemo.Greeter.GreeterClient(channel);

            // make a call
            using (var call = client.SayHello(new HelloRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var reply = call.ResponseStream.Current;
                    Console.WriteLine("Received " + reply.ToString());
                }
            }

            channel.ShutdownAsync().Wait();
        }
    }
}
