using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqRsch
{
	internal class ConsumerContainer
	{
		internal ConsumerContainer(IModel channel, Action<BasicDeliverEventArgs, Action<BasicDeliverEventArgs>> callBack)
		{
			_channel = channel;
			_consumer = new EventingBasicConsumer(_channel);
			_callBack = callBack;
			_consumer.Received += ReceivedHandler;
			_channel.BasicConsume(queue: "hello", noAck: false, consumer: _consumer);
		}

		private void ReceivedHandler(object sender, BasicDeliverEventArgs e)
		{
			_callBack(e, (x) =>
			{
				_consumer.Model.BasicReject(x.DeliveryTag, true);
			});
		}

		private Action<BasicDeliverEventArgs, Action<BasicDeliverEventArgs>> _callBack;
		private readonly IModel _channel;
		private readonly EventingBasicConsumer _consumer;
	}

	public class Program
	{
		private static ConnectionFactory _factory;

		public static void Main(string[] args)
		{

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

			ConsumerContainer[] v = new ConsumerContainer[2];

			using (var connection = _factory.CreateConnection())
			{
				var buf = new List<IModel>();
				for (int i = 0; i < 2; i++)
				{
					var channel = connection.CreateModel();

					channel.QueueDeclare(queue: "hello",
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					buf.Add(channel);

					v[i] = new ConsumerContainer(channel, ProcessMsg);
				}

				Thread.Sleep(TimeSpan.FromSeconds(5));
				foreach (var itr in buf)
				{
					itr.Dispose();
				}
				Console.ReadLine();
			}
		}

		private static void ProcessMsg(BasicDeliverEventArgs ea, Action<BasicDeliverEventArgs> timeoutExpire)
		{
			var rnd = new Random(DateTime.UtcNow.Millisecond);
			var body = ea.Body;
			var message = SimpleSerializer.Deserialize<Message>(body);
			Console.WriteLine("[{0}, ThreadID: {1}] [x] Received {2}", DateTime.UtcNow, 0, message);
			if (rnd.Next(1000) > 800)
			{
				timeoutExpire(ea);
				return;
			}
			Thread.Sleep(rnd.Next(3000));
		}



		private static void SendInLoop(object state)
		{
			var factory = _factory;
			var rnd = new Random();
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					channel.QueueDeclare(queue: "hello",
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					while (true)
					{

						var body = SimpleSerializer.Serialize(new Message { Content = "abc" });

						channel.BasicPublish(exchange: "",
							routingKey: "hello",
							basicProperties: null,
							body: body);
						Console.WriteLine(" [x] Sent");

						Thread.Sleep(rnd.Next(2000));
					}
				}
			}
		}
	}

	static class SimpleSerializer
	{
		public static byte[] Serialize<T>(T obj)
		{
			var serializer = new DataContractSerializer(typeof(T));
			using (var stream = new MemoryStream())
			{
				using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
				{
					serializer.WriteObject(writer, obj);
				}
				return stream.ToArray();
			}
		}

		public static T Deserialize<T>(byte[] data)
		{
			var serializer = new DataContractSerializer(typeof(T));
			using (var stream = new MemoryStream(data))
			{
				using (var reader = XmlDictionaryReader.CreateBinaryReader(
						stream, XmlDictionaryReaderQuotas.Max))
				{
					return (T)serializer.ReadObject(reader);
				}
			}
		}
	}

	[DataContract]
	public class Message
	{
		[DataMember]
		public string Content { get; set; }

		public override string ToString()
		{
			return String.Format("Message: {{ Content: '{0}' }}", Content);
		}
	}
}
