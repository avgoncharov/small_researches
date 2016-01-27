using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqRsch
{
	/// <summary>
	/// Container for event-driven consumer / actor. For push-strategy realization.
	/// </summary>
	public class EventingConsumerContainer: IDisposable
	{
		internal EventingConsumerContainer(IModel channel, string queueName, CancellationToken token, int id)
		{
			_token = token;
			_id = id;
			_channel = channel;
			_channel.BasicQos(0, 1, false);
			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += ReceivedHandler;
			_channel.BasicConsume(queue: queueName, noAck: false, consumer: _consumer);
		}

		private void ReceivedHandler(object sender, BasicDeliverEventArgs e)
		{
			try
			{
				var msg = SimpleSerializer.Deserialize<Message>(e.Body);
				var actor = new PushActor();

				using (var context = new Context(_channel, e))
				{
					actor.Process(msg, context, _id);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			if (_token.IsCancellationRequested)
			{
				_channel.BasicCancel(_consumer.ConsumerTag);
			}
		}

		public void Dispose()
		{
			if (_channel != null)
			{
				try
				{
					_channel.Close();
					_channel.Dispose();
				
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}

				_channel = null;
			}
		}

		private readonly CancellationToken _token;
		private IModel _channel;
		private int _id;
		private readonly EventingBasicConsumer _consumer;
	}
}
