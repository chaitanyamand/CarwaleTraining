using Grpc.Net.Client;
using GrpcDemo;

var channel = GrpcChannel.ForAddress("http://localhost:5180");
var client = new Greeter.GreeterClient(channel);

Console.Write("Enter your name: ");
var name = Console.ReadLine();

var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
Console.WriteLine("Greeting: " + reply.Message);
