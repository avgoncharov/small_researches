using System;
using System.Threading;
using RabbitMqRsch.StatisticApi;
using RabbitMQ.Client;

namespace RabbitMqRsch
{
	/// This program shows (for RabbitMq):
	/// 1) How to get statistic from RabbitMq.
	/// 2) How to create an infrastructure for event-driven (push) strategy working with RabbitMq.
	/// 3) How to create an infrastructure for pull-strategy working with RabbitMq.
	/// 4) Simple example of using generic data contract instead known types.
	public class Program
	{
		public static void Main(string[] args)
		{
			ShowSimpleGenericWrapper();

			_token = Source.Token;

			Console.WriteLine(RabbitMqStatisticGetter.GetStatisticForQueuer("testQueueName_msg"));

			_factory = new ConnectionFactory
			{
				HostName = "localhost",
				Port = 5672,
				UserName = ConnectionFactory.DefaultUser,
				Password = ConnectionFactory.DefaultPass,
				VirtualHost = ConnectionFactory.DefaultVHost,
				Protocol = Protocols.AMQP_0_9_1
			};

			
			ThreadPool.QueueUserWorkItem(SendInLoop);
			
			const int count = 1;
			var v = new EventingConsumerContainer[count];
			var pullActors = new PullActor[count];

			using (var connection = _factory.CreateConnection())
			{
				for (int i = 0; i < count; i++)
				{
					var channel = connection.CreateModel();

					channel.QueueDeclare(queue: "hello",
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					v[i] = new EventingConsumerContainer(channel, QueueName, Source.Token, i);
					
					channel = connection.CreateModel();

					channel.QueueDeclare(queue: QueueName,
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					pullActors[i] = new PullActor(new PullConsumer(channel, QueueName), Source.Token, i);
				}
				
				Console.ReadLine();
				Source.Cancel();
				Thread.Sleep(3000);
			}


		}

		private static void ShowSimpleGenericWrapper()
		{
			// Simple example of using generic data contract instead known types.
			var m = new Message2{Content = "xyz", Count = 1};
			var w = new MessageWrapper<Message2> { Guid = Guid.Empty, Message = m };
			var body = SimpleSerializer.Serialize(w);
			var restore = SimpleSerializer.Deserialize<MessageWrapper<Message2>>(body);
			Console.WriteLine("In restored wrapper we have msg2: {0}.", restore.Message.GetType() == typeof(Message2));
		}

		private static void SendInLoop(object state)
		{
//			var rnd = new Random();
			using (var connection = _factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(
						queue: QueueName,
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					while (!_token.IsCancellationRequested)
					{

						var msg = new Message {Content = "Time: " + DateTime.UtcNow.ToLongTimeString()};
						var body = SimpleSerializer.Serialize(msg);

						channel.BasicPublish(
							exchange: "",
							routingKey: QueueName,
							basicProperties: null,
							body: body);
						Console.WriteLine(" [x] Sent {0}", msg);

						//Thread.Sleep(rnd.Next(500, 3000));
						Thread.Sleep(500);
					}
				}
			}
		}

		private static CancellationToken _token;
		private static readonly CancellationTokenSource Source = new CancellationTokenSource();
		private static ConnectionFactory _factory;
		private const string QueueName = "hello";
	}
}
